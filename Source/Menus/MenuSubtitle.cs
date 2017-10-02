namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;

    public class MenuSubtitle : IMenuComponent
    {
        private int counterTotalCount = 0, counterOnScreenSelectedIndex = 0;

        public Menu Menu { get; private set; }
        public virtual string Text { get; set; }

        public MenuSubtitle(string text)
        {
            Text = text;
        }

        public MenuSubtitle() : this(null)
        {
        }

        ~MenuSubtitle()
        {
            if (Menu != null)
            {
                Menu.SelectedIndexChanged -= OnMenuSelectedIndexChanged;
            }
        }

        internal void SetMenu(Menu menu)
        {
            if (Menu != null && Menu != menu)
                throw new System.InvalidOperationException($"{nameof(MenuSubtitle)} already set to a {nameof(Menus.Menu)}.");
            Menu = menu ?? throw new System.ArgumentNullException($"The {nameof(MenuSubtitle)} {nameof(Menu)} can't be null.");
            Menu.SelectedIndexChanged -= OnMenuSelectedIndexChanged; // remove first in case it's set to the same menu twice
            Menu.SelectedIndexChanged += OnMenuSelectedIndexChanged;
        }

        protected internal virtual string GetItemsCounterText()
        {
            if ((counterTotalCount == 0 && counterOnScreenSelectedIndex == 0) || counterTotalCount != Menu.Items.Count)
                UpdateCounter();
            return $"{counterOnScreenSelectedIndex}/{counterTotalCount} ";
        }

        public virtual void Process()
        {
        }

        public virtual void Draw(Graphics graphics, ref float x, ref float y)
        {
            Menu.Style.DrawSubtitle(graphics, this, ref x, ref y);
        }

        private void UpdateCounter()
        {
            counterTotalCount = 0;
            counterOnScreenSelectedIndex = 0;

            int realSelectedIndex = Menu.SelectedIndex;

            for (int i = 0; i < Menu.Items.Count; i++)
            {
                MenuItem item = Menu.Items[i];

                if (item != null && item.IsVisible)
                {
                    counterTotalCount++;
                    if (i == realSelectedIndex)
                        counterOnScreenSelectedIndex = counterTotalCount;
                }
            }
        }

        private void OnMenuSelectedIndexChanged(Menu sender, int oldIndex, int newIndex)
        {
            UpdateCounter();
        }
    }
}

