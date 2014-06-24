namespace Arya.Framework.Collections.Generic
{
    public class DoubleKeyPairValue<TK1, TK2, TV>
    {
        #region Constructors (1)

        public DoubleKeyPairValue(TK1 key1, TK2 key2, TV value)
        {
            Key1 = key1;
            Key2 = key2;
            Value = value;
        }

        #endregion Constructors

        #region Properties (3)

        public TK1 Key1 { get; set; }

        public TK2 Key2 { get; set; }

        public TV Value { get; set; }

        #endregion Properties

        #region Methods (1)

        // Public Methods (1) 

        public override string ToString()
        {
            return Key1 + " - " + Key2 + " - " + Value;
        }

        #endregion Methods
    }
}
