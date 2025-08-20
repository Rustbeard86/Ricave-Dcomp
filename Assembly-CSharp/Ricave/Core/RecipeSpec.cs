using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class RecipeSpec : Spec
    {
        public List<EntitySpec> Ingredients
        {
            get
            {
                return this.ingredients;
            }
        }

        public EntitySpec Product
        {
            get
            {
                return this.product;
            }
        }

        public float UIOrder
        {
            get
            {
                return this.uiOrder;
            }
        }

        public int ManaIngredient
        {
            get
            {
                return this.manaIngredient;
            }
        }

        public RecipeSpec.Category RecipeCategory
        {
            get
            {
                return this.category;
            }
        }

        [Saved(Default.New, false)]
        private List<EntitySpec> ingredients = new List<EntitySpec>();

        [Saved]
        private EntitySpec product;

        [Saved]
        private float uiOrder;

        [Saved]
        private int manaIngredient;

        [Saved]
        private RecipeSpec.Category category;

        public enum Category
        {
            Utilities,

            Potions,

            Scrolls,

            WeaponsAndGear
        }
    }
}