﻿namespace RNUIExamples.Showcase
{
    using System;
    using System.Drawing;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Rage;

    using static Util;

    internal sealed class Menus : UIMenu
    {
        public Menus() : base(Plugin.MenuTitle, "MENUS")
        {
            Plugin.Pool.Add(this);

            CreateMenuItems();
        }

        private void CreateMenuItems()
        {
            UIMenuItem visualOptionsBindItem = new UIMenuItem("Visual Options", $"Demonstrates the visual options of the ~b~{nameof(UIMenu)}~s~ class.");
            UIMenuItem checkboxBindItem = new UIMenuItem("Checkbox", $"Demonstrates the ~b~{nameof(UIMenuCheckboxItem)}~s~ class.");
            UIMenuItem scrollerBindItem = new UIMenuItem("Scroller", $"Demonstrates the ~b~{nameof(UIMenuListScrollerItem<int>)}~s~ and ~b~{nameof(UIMenuNumericScrollerItem<int>)}~s~ classes.");

            UIMenu visualOptionsMenu = new UIMenu(Title, Subtitle + ": VISUAL OPTIONS");
            UIMenu checkboxMenu = new UIMenu(Title, Subtitle + ": CHECKBOX");
            UIMenu scrollerMenu = new UIMenu(Title, Subtitle + ": SCROLLER");

            Plugin.Pool.Add(visualOptionsMenu, checkboxMenu, scrollerMenu);

            AddItems(visualOptionsBindItem, checkboxBindItem, scrollerBindItem);

            BindMenuToItem(visualOptionsMenu, visualOptionsBindItem);
            BindMenuToItem(checkboxMenu, checkboxBindItem);
            BindMenuToItem(scrollerMenu, scrollerBindItem);

            // visual options
            {
                static void ApplyToEachMenu(Action<UIMenu> f)
                {
                    foreach (UIMenu m in Plugin.Pool)
                    {
                        f(m);
                    }
                }

                var width = new UIMenuNumericScrollerItem<float>(nameof(Width), $"Modifies the ~b~{nameof(UIMenu)}.{nameof(Width)}~s~ property.", 0.0f, 1.0f, 0.001f);
                width.Formatter = v => v.ToString("0.000");
                width.Value = Width;
                width.IndexChanged += (s, o, n) => ApplyToEachMenu(m => m.Width = width.Value);

                var offsetX = new UIMenuNumericScrollerItem<int>("Offset X", $"Modifies the X-coord of the ~b~{nameof(UIMenu)}.{nameof(Offset)}~s~ property.", -1000, 1000, 1);
                offsetX.Value = Offset.X;
                offsetX.IndexChanged += (s, o, n) => ApplyToEachMenu(m => m.Offset = new Point(offsetX.Value, m.Offset.Y));

                var offsetY = new UIMenuNumericScrollerItem<int>("Offset Y", $"Modifies the Y-coord of the ~b~{nameof(UIMenu)}.{nameof(Offset)}~s~ property.", -1000, 1000, 1);
                offsetY.Value = Offset.Y;
                offsetY.IndexChanged += (s, o, n) => ApplyToEachMenu(m => m.Offset = new Point(m.Offset.X, offsetY.Value));

                const string BannerTypeText = "Banner Type (Current)", BannerTypeUnsetText = "Banner Type";
                var bannerType = new UIMenuListScrollerItem<string>(BannerTypeText, "", new[] { "No Banner", "Sprite", "Color", "Custom Texture"});
                int currBannerType = bannerType.Index = 1;
                bannerType.IndexChanged += (s, o, n) => bannerType.Text = n == currBannerType ? BannerTypeText : BannerTypeUnsetText;
                bannerType.Activated += (m, s) =>
                {
                    currBannerType = bannerType.Index;
                    bannerType.Text = BannerTypeText;
                    ApplyToEachMenu(currBannerType switch
                    {
                        0 => m => m.RemoveBanner(),
                        1 => m => m.SetBannerType(new Sprite("commonmenu", "interaction_bgd", Point.Empty, Size.Empty)),
                        2 => m => m.SetBannerType(new ResRectangle(Point.Empty, Size.Empty, HudColor.Red.GetColor())),
                        3 => m => m.SetBannerType(Game.CreateTextureFromFile("cursor_32_2.png")),
                        _ => throw new InvalidOperationException("Invalid banner type")
                    });
                };
                
                var title = NewTextEditingItem(nameof(Title), $"Select to modify the ~b~{nameof(UIMenu)}.{nameof(Title)}~s~ property.",
                                               () => Title,
                                               s => ApplyToEachMenu(m => m.Title = s));

                var titleColor = NewColorsItem("Title Color", $"Modifies the color of the ~b~{nameof(UIMenu)}.{nameof(TitleStyle)}~s~ property.");
                titleColor.IndexChanged += (s, o, n) =>
                {
                    Color c = titleColor.SelectedItem.GetColor();
                    ApplyToEachMenu(m => m.TitleStyle = m.TitleStyle.With(color: c));
                };

                var subtitleColor = NewColorsItem("Subtitle Color", $"Modifies the color of the ~b~{nameof(UIMenu)}.{nameof(SubtitleStyle)}~s~ property.");
                subtitleColor.IndexChanged += (s, o, n) =>
                {
                    Color c = subtitleColor.SelectedItem.GetColor();
                    ApplyToEachMenu(m => m.SubtitleStyle = m.SubtitleStyle.With(color: c));
                };

                var descriptionOverride = NewTextEditingItem("Description Override", $"Select to modify the ~b~{nameof(UIMenu)}.{nameof(DescriptionOverride)}~s~ property.",
                                               () => DescriptionOverride ?? string.Empty,
                                               s => ApplyToEachMenu(m => m.DescriptionOverride = s.Length == 0 ? null : s));

                var descriptionColor = NewColorsItem("Description Color", $"Modifies the color of the ~b~{nameof(UIMenu)}.{nameof(DescriptionStyle)}~s~ property.");
                descriptionColor.IndexChanged += (s, o, n) =>
                {
                    Color c = descriptionColor.SelectedItem.GetColor();
                    ApplyToEachMenu(m => m.DescriptionStyle = m.DescriptionStyle.With(color: c));
                };

                var descriptionSeparatorColor = NewColorsItem("Description Separator", $"Modifies the ~b~{nameof(UIMenu)}.{nameof(DescriptionSeparatorColor)}~s~ property.");
                descriptionSeparatorColor.SelectedItem = HudColor.Black;
                descriptionSeparatorColor.IndexChanged += (s, o, n) =>
                {
                    Color c = descriptionSeparatorColor.SelectedItem.GetColor();
                    ApplyToEachMenu(m => m.DescriptionSeparatorColor = c);
                };

                var counterOverride = NewTextEditingItem("Counter Override", $"Select to modify the ~b~{nameof(UIMenu)}.{nameof(CounterOverride)}~s~ property.",
                                               () => CounterOverride ?? string.Empty,
                                               s => ApplyToEachMenu(m => m.CounterOverride = s.Length == 0 ? null : s));

                var counterColor = NewColorsItem("Counter Color", $"Modifies the color of the ~b~{nameof(UIMenu)}.{nameof(CounterStyle)}~s~ property.");
                counterColor.IndexChanged += (s, o, n) =>
                {
                    Color c = counterColor.SelectedItem.GetColor();
                    ApplyToEachMenu(m => m.CounterStyle = m.CounterStyle.With(color: c));
                };

                var scaleWithSafezone = new UIMenuCheckboxItem("Scale with Safe-Zone", ScaleWithSafezone, $"Modifies the ~b~{nameof(UIMenu)}.{nameof(ScaleWithSafezone)}~s~ property.");
                scaleWithSafezone.Checked = ScaleWithSafezone;
                scaleWithSafezone.CheckboxEvent += (s, v) => ApplyToEachMenu(m => m.ScaleWithSafezone = v);

                visualOptionsMenu.AddItems(width, offsetX, offsetY, bannerType, title, titleColor, subtitleColor,
                                           descriptionOverride, descriptionColor, descriptionSeparatorColor,
                                           counterOverride, counterColor, scaleWithSafezone);
            }

            // checkbox
            {
                checkboxMenu.AddItems(
                    new UIMenuCheckboxItem("#1", false, "Unchecked."),
                    new UIMenuCheckboxItem("#2", true, "Checked."),
                    new UIMenuCheckboxItem("#3", false, "Unchecked and disabled.") { Enabled = false },
                    new UIMenuCheckboxItem("#4", true, "Checked and disabled.") { Enabled = false },
                    new UIMenuCheckboxItem("#5", false, "With right badge.") { RightBadge = UIMenuItem.BadgeStyle.Armour },
                    new UIMenuCheckboxItem("#6", true, "With right badge.") { RightBadge = UIMenuItem.BadgeStyle.Armour },
                    new UIMenuCheckboxItem("#7", false, "With left badge.") { LeftBadge = UIMenuItem.BadgeStyle.Armour },
                    new UIMenuCheckboxItem("#8", true, "With left badge.") { LeftBadge = UIMenuItem.BadgeStyle.Armour },
                    new UIMenuCheckboxItem("#9", false, "With both badges.") { RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour },
                    new UIMenuCheckboxItem("#10", true, "With both badges.") { RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour },
                    new UIMenuCheckboxItem("#11", false, "With both badges and disabled.") { Enabled = false, RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour },
                    new UIMenuCheckboxItem("#12", true, "With both badges and disabled.") { Enabled = false, RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour },
                    new UIMenuCheckboxItem("#13", false, "With custom background color.") { BackColor = Color.FromArgb(140, HudColor.RedDark.GetColor()), HighlightedBackColor = Color.FromArgb(230, HudColor.TechRed.GetColor()) },
                    new UIMenuCheckboxItem("#14", true, "With custom background color.") { BackColor = Color.FromArgb(140, HudColor.RedDark.GetColor()), HighlightedBackColor = Color.FromArgb(230, HudColor.TechRed.GetColor()) },
                    new UIMenuCheckboxItem("#14", false, "With custom badge.") { RightBadgeInfo = new UIMenuItem.BadgeInfo("commonmenu", "mp_alerttriangle", HudColor.Red.GetColor()) },
                    new UIMenuCheckboxItem("#15", true, "With custom badge.") { RightBadgeInfo = new UIMenuItem.BadgeInfo("commonmenu", "mp_alerttriangle", HudColor.Red.GetColor()) }
                );
            }

            // scroller
            {
                string[] values = new[] { "Hello", "World", "Foo", "Bar" };
                scrollerMenu.AddItems(
                    new UIMenuListScrollerItem<string>("List #1", "List scroller.", values),
                    new UIMenuListScrollerItem<string>("List #2", "List scroller disabled.", values) { Enabled = false },
                    new UIMenuListScrollerItem<string>("List #3", "List scroller disabled with scrolling enabled.", values) { Enabled = false, ScrollingEnabledWhenDisabled = true },
                    new UIMenuListScrollerItem<string>("List #4", "List scroller with scrolling disabled.", values) { ScrollingEnabled = false },
                    new UIMenuListScrollerItem<string>("List #5", "List scroller without wrap around.", values) { AllowWrapAround = false },
                    new UIMenuListScrollerItem<string>("List #6", "List scroller with right badge.", values) { RightBadge = UIMenuItem.BadgeStyle.Car },
                    new UIMenuListScrollerItem<string>("List #7", "List scroller with left badge.", values) { LeftBadge = UIMenuItem.BadgeStyle.Car },
                    new UIMenuListScrollerItem<string>("List #8", "List scroller with both badges.", values) { RightBadge = UIMenuItem.BadgeStyle.Car, LeftBadge = UIMenuItem.BadgeStyle.Car },
                    new UIMenuNumericScrollerItem<int>("Numeric #1", "Numeric scroller.", -10, 10, 1),
                    new UIMenuNumericScrollerItem<int>("Numeric #2", "Numeric scroller disabled.", -10, 10, 1) { Enabled = false },
                    new UIMenuNumericScrollerItem<int>("Numeric #3", "Numeric scroller disabled with scrolling enabled.", -10, 10, 1) { Enabled = false, ScrollingEnabledWhenDisabled = true },
                    new UIMenuNumericScrollerItem<int>("Numeric #4", "Numeric scroller with scrolling disabled.", -10, 10, 1) { ScrollingEnabled = false },
                    new UIMenuNumericScrollerItem<int>("Numeric #5", "Numeric scroller without wrap around.", -10, 10, 1) { AllowWrapAround = false },
                    new UIMenuNumericScrollerItem<int>("Numeric #6", "Numeric scroller with right badge.", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car },
                    new UIMenuNumericScrollerItem<int>("Numeric #7", "Numeric scroller with left badge.", -10, 10, 1) { LeftBadge = UIMenuItem.BadgeStyle.Car },
                    new UIMenuNumericScrollerItem<int>("Numeric #8", "Numeric scroller with both badges.", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car, LeftBadge = UIMenuItem.BadgeStyle.Car },
                    new UIMenuNumericScrollerItem<int>("Slider bar #1", "Scroller with slider bar.", -10, 10, 1) { SliderBar = new UIMenuScrollerSliderBar() },
                    new UIMenuNumericScrollerItem<int>("Slider bar #2", "Scroller with slider bar disabled.", -10, 10, 1) { Enabled = false, SliderBar = new UIMenuScrollerSliderBar() },
                    new UIMenuNumericScrollerItem<int>("Slider bar #3", "Scroller with slider bar disabled with scrolling enabled.", -10, 10, 1) { Enabled = false, ScrollingEnabledWhenDisabled = true, SliderBar = new UIMenuScrollerSliderBar() },
                    new UIMenuNumericScrollerItem<int>("Slider bar #4", "Scroller with slider bar with scrolling disabled.", -10, 10, 1) { ScrollingEnabled = false, SliderBar = new UIMenuScrollerSliderBar() },
                    new UIMenuNumericScrollerItem<int>("Slider bar #5", "Scroller with slider bar without wrap around.", -10, 10, 1) { AllowWrapAround = false, SliderBar = new UIMenuScrollerSliderBar() },
                    new UIMenuNumericScrollerItem<int>("Slider bar #6", "Scroller with slider bar with right badge.", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car, SliderBar = new UIMenuScrollerSliderBar() },
                    new UIMenuNumericScrollerItem<int>("Slider bar #7", "Scroller with slider bar with left badge.", -10, 10, 1) { LeftBadge = UIMenuItem.BadgeStyle.Car, SliderBar = new UIMenuScrollerSliderBar() },
                    new UIMenuNumericScrollerItem<int>("Slider bar #8", "Scroller with slider bar with both badges.", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car, LeftBadge = UIMenuItem.BadgeStyle.Car, SliderBar = new UIMenuScrollerSliderBar() },
                    new UIMenuNumericScrollerItem<int>("Slider bar #9", "Scroller with slider bar with custom colors.", -10, 10, 1)
                    { 
                        BackColor = Color.FromArgb(190, HudColor.BlueDark.GetColor()),
                        HighlightedBackColor = Color.FromArgb(220, HudColor.Blue.GetColor()),
                        SliderBar = new UIMenuScrollerSliderBar() { ForegroundColor = HudColor.Purple.GetColor(), BackgroundColor = Color.FromArgb(120, HudColor.Purple.GetColor()) }
                    },
                    new UIMenuNumericScrollerItem<int>("Slider bar #10", "Scroller with slider bar with full width and height and custom color.", -10, 10, 1)
                    { 
                        SliderBar = new UIMenuScrollerSliderBar() { Width = 1.0f, Height = 1.0f, ForegroundColor = HudColor.Red.GetColor(), BackgroundColor = Color.FromArgb(120, HudColor.Red.GetColor()) }
                    },
                    new UIMenuNumericScrollerItem<int>("Slider bar #11", "Scroller with slider bar with half width and height and custom color.", -10, 10, 1)
                    { 
                        SliderBar = new UIMenuScrollerSliderBar() { Width = 0.5f, Height = 0.5f, ForegroundColor = HudColor.Green.GetColor(), BackgroundColor = Color.FromArgb(120, HudColor.Green.GetColor()) }
                    }
                );
            }
        }
    }
}
