namespace RAGENativeUI.Menus
{
    using System;
    using System.Globalization;

    using Rage;

    public class MenuItemNumericScroller : MenuItemScroller
    {
        private decimal currentValue;
        public decimal Value
        {
            get { return Minimum + (Increment * SelectedIndex); }
            set
            {
                if(value != currentValue)
                {
                    if(value < minimun || value > maximun)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Value), $"{nameof(Value)} can't be lower than {nameof(Minimum)} or higher than {nameof(Maximum)}.");
                    }

                    currentValue = value;
                }
            }
        }


        private decimal minimun = 0.0m;
        public decimal Minimum
        {
            get { return minimun; }
            set
            {
                minimun = value;
                if (minimun > maximun)
                {
                    maximun = minimun;
                }

                Value = EnsureValue(Value);
            }
        }

        private decimal maximun = 100.0m;
        public decimal Maximum
        {
            get { return maximun; } 
            set
            {
                maximun = value;
                if(minimun > maximun)
                {
                    minimun = maximun;
                }

                Value = EnsureValue(Value);
            }
        }

        private decimal increment = 0.5m;
        public decimal Increment
        {
            get { return increment; }
            set
            {
                if(value < 0.0m)
                {
                    throw new ArgumentOutOfRangeException(nameof(Increment), $"{nameof(Increment)} can't be lower than zero.");
                }

                increment = value;
            }
        }

        public bool ThousandsSeparator { get; set; }
        public bool Hexadecimal { get; set; }
        public int DecimalPlaces { get; set; } = 2;

        public MenuItemNumericScroller(string text) : base(text)
        {
            SelectedIndex = GetOptionsCount() / 2;
        }

        private decimal EnsureValue(decimal value)
        {
            if (value < minimun)
                value = minimun;

            if (value > maximun)
                value = maximun;

            return value;

        }

        protected override int GetOptionsCount()
        {
            return (int)(Math.Abs(Minimum) + Math.Abs(Maximum) / Increment);
        }

        protected override string GetSelectedOptionText()
        {
            string text;

            if (Hexadecimal)
            {
                text = ((Int64)currentValue).ToString("X", CultureInfo.InvariantCulture);
            }
            else
            {
                text = currentValue.ToString((ThousandsSeparator ? "N" : "F") + DecimalPlaces.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
            }

            return text;
        }

        protected internal override bool OnPreviewMoveLeft(Menu menuSender)
        {
            base.OnPreviewMoveLeft(menuSender);

            decimal newValue = currentValue;

            try
            {
                newValue -= Increment;

                if (newValue < minimun)
                    newValue = minimun;
            }
#if DEBUG
            catch (OverflowException ex)
            {
                Game.LogTrivial("MenuItemNumericScroller.OnPreviewMoveLeft: OverflowException");
                Game.LogTrivial(ex.ToString());

                newValue = minimun;
            }
#else
            catch (OverflowException)
            {
                newValue = minimum;
            }
#endif
            Value = newValue;

            return true;
        }

        protected internal override bool OnPreviewMoveRight(Menu menuSender)
        {
            base.OnPreviewMoveLeft(menuSender);

            decimal newValue = currentValue;

            try
            {
                newValue += Increment;

                if (newValue > maximun)
                    newValue = maximun;
            }
#if DEBUG
            catch (OverflowException ex)
            {
                Game.LogTrivial("MenuItemNumericScroller.OnPreviewMoveLeft: OverflowException");
                Game.LogTrivial(ex.ToString());

                newValue = maximun;
            }
#else
            catch (OverflowException)
            {
                newValue = maximun;
            }
#endif
            Value = newValue;

            return true;
        }
        /*
         0 -> Min
         (int)(Math.Abs(Minimum) + Math.Abs(Maximum) / Increment) -> Max
         
         */
    }
}
