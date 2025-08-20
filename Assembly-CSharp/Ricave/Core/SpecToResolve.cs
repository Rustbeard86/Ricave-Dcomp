using System;

namespace Ricave.Core
{
    public struct SpecToResolve
    {
        public Type SpecType
        {
            get
            {
                return this.specType;
            }
        }

        public string SpecID
        {
            get
            {
                return this.specID;
            }
        }

        public SpecToResolve(Type specType, string specID, Action<object> specAssigner)
        {
            this.specType = specType;
            this.specID = specID;
            this.specAssigner = specAssigner;
        }

        public void Assign(Spec spec)
        {
            this.specAssigner(spec);
        }

        private Type specType;

        private string specID;

        private Action<object> specAssigner;
    }
}