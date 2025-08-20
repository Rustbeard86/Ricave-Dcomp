using System;
using UnityEngine;

namespace Ricave.Core
{
    public struct Target : IEquatable<Target>
    {
        public bool IsValid
        {
            get
            {
                return this.IsEntity || this.IsLocation;
            }
        }

        public bool IsEntity
        {
            get
            {
                return this.entity != null;
            }
        }

        public bool IsLocation
        {
            get
            {
                return this.location != null;
            }
        }

        public bool Spawned
        {
            get
            {
                return (this.IsEntity && this.entity.Spawned) || this.IsLocation;
            }
        }

        public int MyStableHash
        {
            get
            {
                Entity entity = this.entity;
                if (entity != null)
                {
                    return entity.MyStableHash;
                }
                if (this.location == null)
                {
                    return 912107461;
                }
                return this.location.GetValueOrDefault().GetHashCode();
            }
        }

        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public Vector3Int Position
        {
            get
            {
                Entity entity = this.entity;
                if (entity != null)
                {
                    return entity.Position;
                }
                Vector3Int? vector3Int = this.location;
                if (vector3Int == null)
                {
                    return Vector3Int.zero;
                }
                return vector3Int.GetValueOrDefault();
            }
        }

        public Vector3 RenderPosition
        {
            get
            {
                Entity entity = this.entity;
                if (entity != null)
                {
                    return entity.RenderPosition;
                }
                Vector3Int? vector3Int = this.location;
                if (vector3Int == null)
                {
                    return Vector3.zero;
                }
                return vector3Int.GetValueOrDefault();
            }
        }

        public bool IsMainActor
        {
            get
            {
                return this.IsEntity && this.entity.IsMainActor;
            }
        }

        public bool IsNowControlledActor
        {
            get
            {
                return this.IsEntity && this.entity.IsNowControlledActor;
            }
        }

        public bool IsPlayerParty
        {
            get
            {
                return this.IsEntity && this.entity.IsPlayerParty;
            }
        }

        public bool IsNowControlledPlayerParty
        {
            get
            {
                return this.IsEntity && this.entity.IsNowControlledPlayerParty;
            }
        }

        public Target(Entity entity)
        {
            this.entity = entity;
            this.location = null;
        }

        public Target(Vector3Int location)
        {
            this.entity = null;
            this.location = new Vector3Int?(location);
        }

        public object ToObject()
        {
            if (this.entity != null)
            {
                return this.entity;
            }
            if (!this.IsValid)
            {
                return Target.EmptyTargetBoxed;
            }
            return this;
        }

        public static implicit operator Target(Entity entity)
        {
            return new Target(entity);
        }

        public static implicit operator Target(Vector3Int location)
        {
            return new Target(location);
        }

        public static bool operator ==(Target target, Entity entity)
        {
            return !target.IsLocation && target.Entity == entity;
        }

        public static bool operator !=(Target target, Entity entity)
        {
            return !(target == entity);
        }

        public static bool operator ==(Target target, Vector3Int location)
        {
            return target.IsLocation && target.Position == location;
        }

        public static bool operator !=(Target target, Vector3Int location)
        {
            return !(target == location);
        }

        public static bool operator ==(Target target, Target other)
        {
            return target.Entity == other.Entity && target.location == other.location;
        }

        public static bool operator !=(Target target, Target other)
        {
            return !(target == other);
        }

        public override bool Equals(object other)
        {
            if (this == other)
            {
                return true;
            }
            Entity entity = other as Entity;
            if (entity != null)
            {
                return this.Equals(entity);
            }
            if (other is Target)
            {
                Target target = (Target)other;
                return this.Equals(target);
            }
            if (other is Vector3Int)
            {
                Vector3Int vector3Int = (Vector3Int)other;
                return this.Equals(vector3Int);
            }
            return false;
        }

        public bool Equals(Entity entity)
        {
            return this == entity;
        }

        public bool Equals(Vector3Int location)
        {
            return this == location;
        }

        public bool Equals(Target other)
        {
            return this == other;
        }

        public static explicit operator Entity(Target target)
        {
            return target.Entity;
        }

        public static explicit operator Vector3Int(Target target)
        {
            return target.Position;
        }

        public override int GetHashCode()
        {
            return Calc.CombineHashes<Entity, Vector3Int?>(this.entity, this.location);
        }

        public override string ToString()
        {
            if (this.entity != null)
            {
                return this.entity.ToString();
            }
            if (this.location != null)
            {
                return this.location.Value.ToString();
            }
            return "none";
        }

        [Saved]
        private Entity entity;

        [Saved]
        private Vector3Int? location;

        private static readonly object EmptyTargetBoxed = new Target(null);
    }
}