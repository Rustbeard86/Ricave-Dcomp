using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class GenPassSpec : Spec
    {
        public GenPass GenPass
        {
            get
            {
                return this.genPass;
            }
        }

        public float Order
        {
            get
            {
                return this.order;
            }
        }

        public List<WorldSpec> AddToWorldSpecs
        {
            get
            {
                return this.addToWorldSpecs;
            }
        }

        [Saved]
        private GenPass genPass;

        [Saved]
        private float order;

        [Saved(Default.New, false)]
        private List<WorldSpec> addToWorldSpecs = new List<WorldSpec>();
    }
}