using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class PlaceManager : ISaveableEventsReceiver
    {
        public List<Place> Places
        {
            get
            {
                return this.places;
            }
        }

        public List<PlaceLink> Links
        {
            get
            {
                return this.links;
            }
        }

        public bool Contains(Place place)
        {
            return place != null && this.places.Contains(place);
        }

        public bool Contains_ProbablyAt(Place place, int probablyAt)
        {
            return place != null && this.places.Contains_ProbablyAt(place, probablyAt);
        }

        public void AddPlace(Place place, int insertAt = -1)
        {
            Instruction.ThrowIfNotExecuting();
            if (place == null)
            {
                Log.Error("Tried to add null place.", false);
                return;
            }
            if (this.Contains(place))
            {
                Log.Error("Tried to add the same place twice.", false);
                return;
            }
            if (insertAt < 0)
            {
                this.places.Add(place);
                return;
            }
            if (insertAt > this.places.Count)
            {
                Log.Error(string.Concat(new string[]
                {
                    "Tried to insert place at index ",
                    insertAt.ToString(),
                    " but places count is only ",
                    this.places.Count.ToString(),
                    "."
                }), false);
                return;
            }
            this.places.Insert(insertAt, place);
        }

        public int RemovePlace(Place place)
        {
            Instruction.ThrowIfNotExecuting();
            if (place == null)
            {
                Log.Error("Tried to remove null place.", false);
                return -1;
            }
            if (!this.Contains(place))
            {
                Log.Error("Tried to remove place but it's not here.", false);
                return -1;
            }
            int num = this.places.IndexOf(place);
            this.places.RemoveAt(num);
            return num;
        }

        public Place GetFirstOfSpec(PlaceSpec spec)
        {
            for (int i = 0; i < this.places.Count; i++)
            {
                if (this.places[i].Spec == spec)
                {
                    return this.places[i];
                }
            }
            return null;
        }

        public bool AnyOfSpec(PlaceSpec spec)
        {
            return this.GetFirstOfSpec(spec) != null;
        }

        public PlaceLink GetLink(Place from, Place to)
        {
            if (from == null || to == null)
            {
                return null;
            }
            if (from == to)
            {
                return null;
            }
            int i = 0;
            int count = this.links.Count;
            while (i < count)
            {
                PlaceLink placeLink = this.links[i];
                if (placeLink.From == from && placeLink.To == to)
                {
                    return placeLink;
                }
                i++;
            }
            return null;
        }

        public IEnumerable<PlaceLink> GetLinksFrom(Place from)
        {
            if (from == null)
            {
                yield break;
            }
            int i = 0;
            int count = this.links.Count;
            while (i < count)
            {
                PlaceLink placeLink = this.links[i];
                if (placeLink.From == from)
                {
                    yield return placeLink;
                }
                int num = i;
                i = num + 1;
            }
            yield break;
        }

        public bool LinkExists(Place from, Place to)
        {
            return this.GetLink(from, to) != null;
        }

        public bool ContainsLink(PlaceLink link)
        {
            return link != null && this.links.Contains(link);
        }

        public void AddLink(PlaceLink link, int insertAt = -1)
        {
            Instruction.ThrowIfNotExecuting();
            if (link == null)
            {
                Log.Error("Tried to add null link.", false);
                return;
            }
            if (this.LinkExists(link.From, link.To))
            {
                Log.Error("Tried to add the same link twice.", false);
                return;
            }
            if (insertAt < 0)
            {
                this.links.Add(link);
                return;
            }
            if (insertAt > this.links.Count)
            {
                Log.Error(string.Concat(new string[]
                {
                    "Tried to insert link at index ",
                    insertAt.ToString(),
                    " but list count is only ",
                    this.links.Count.ToString(),
                    "."
                }), false);
                return;
            }
            this.links.Insert(insertAt, link);
        }

        public int RemoveLink(PlaceLink link)
        {
            Instruction.ThrowIfNotExecuting();
            if (link == null)
            {
                Log.Error("Tried to remove null link.", false);
                return -1;
            }
            if (!this.ContainsLink(link))
            {
                Log.Error("Tried to remove link but it's not here.", false);
                return -1;
            }
            int num = this.links.IndexOf(link);
            this.links.Remove(link);
            return num;
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.places.RemoveAll((Place x) => x.Spec == null) != 0)
            {
                Log.Error("Removed some places with null spec from PlaceManager.", false);
            }
            if (this.links.RemoveAll((PlaceLink x) => x.From == null || x.To == null) != 0)
            {
                Log.Error("Removed some links with null places from PlaceManager.", false);
            }
        }

        public IEnumerable<Place> GetNextPlaces(Place fromPlace)
        {
            if (fromPlace == null)
            {
                yield break;
            }
            int i = 0;
            int count = this.links.Count;
            while (i < count)
            {
                PlaceLink placeLink = this.links[i];
                if (placeLink.From == fromPlace)
                {
                    yield return placeLink.To;
                }
                int num = i;
                i = num + 1;
            }
            yield break;
        }

        public Place GetRandomNextNonShelterPlaceOrNull(Place fromPlace)
        {
            if (fromPlace == null)
            {
                return null;
            }
            List<Place> list = this.GetNextPlaces(fromPlace).ToList<Place>();
            if (list.Count == 0)
            {
                return null;
            }
            Place place;
            if (list.Where<Place>((Place x) => x.Spec != Get.Place_Shelter).TryGetRandomElement<Place>(out place))
            {
                return place;
            }
            Place place2;
            list.TryGetRandomElement<Place>(out place2);
            if (this.GetNextPlaces(place2).TryGetRandomElement<Place>(out place))
            {
                return place;
            }
            return null;
        }

        public IEnumerable<Place> GetPreviousPlaces(Place fromPlace)
        {
            if (fromPlace == null)
            {
                yield break;
            }
            int i = 0;
            int count = this.links.Count;
            while (i < count)
            {
                PlaceLink placeLink = this.links[i];
                if (placeLink.To == fromPlace)
                {
                    yield return placeLink.From;
                }
                int num = i;
                i = num + 1;
            }
            yield break;
        }

        public Place GetRandomInitialPlace()
        {
            Place place;
            if (this.places.TryGetRandomElementWhere<Place>((Place x) => !this.GetPreviousPlaces(x).Any<Place>(), out place))
            {
                return place;
            }
            return null;
        }

        public Place GetRandomPlaceForFloor_Debug(int floor)
        {
            Place place;
            if (this.places.Where<Place>((Place x) => x.Floor == floor && x.Spec != Get.Place_Shelter).TryGetRandomElement<Place>(out place))
            {
                return place;
            }
            return null;
        }

        [Saved(Default.New, true)]
        private List<Place> places = new List<Place>();

        [Saved(Default.New, true)]
        private List<PlaceLink> links = new List<PlaceLink>();
    }
}