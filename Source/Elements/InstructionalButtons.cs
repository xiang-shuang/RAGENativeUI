namespace RAGENativeUI.Elements
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Rage;
    using System.Drawing;

    public class InstructionalButtons
    {
        private const string ScaleformName = "instructional_buttons";

        public static readonly Color DefaultBackgroundColor = Color.FromArgb(80, 0, 0, 0);

        private bool needsUpdate = true;

        public Scaleform Scaleform { get; }
        public InstructionalButtonsCollection Buttons { get; }
        public Color BackgroundColor { get; set; } = DefaultBackgroundColor;

        public InstructionalButtons()
        {
            Scaleform = new Scaleform();

            Buttons = new InstructionalButtonsCollection();
            Buttons.ItemAdded += (c, i) => Update();
            Buttons.ItemRemoved += (c, i) => Update();
            Buttons.ItemModified += (c, o, n) => Update();
            Buttons.Cleared += (c) => Update();
        }

        public void Draw()
        {
            if (needsUpdate)
            {
                DoUpdate();
            }

            Scaleform.Render2D();
        }

        public void Update() => needsUpdate = true;

        private void DoUpdate()
        {
            if (!Scaleform.IsLoaded)
            {
                Scaleform.Load(ScaleformName);
                return;
            }

            Scaleform.CallFunction("CLEAR_ALL");
            Scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", true);

            for (int i = 0, slot = 0; i < Buttons.Count; i++)
            {
                IInstructionalButtonSlot b = Buttons[i];
                if (b.CanBeDisplayed == null || b.CanBeDisplayed(b))
                {
                    Scaleform.CallFunction("SET_DATA_SLOT", slot++,
                                           b.GetButtonId() ?? string.Empty,
                                           b.Text ?? string.Empty,
                                           b.BindedControl.HasValue, // clickable?
                                           b.BindedControl.HasValue ? (int)b.BindedControl.Value : - 1); // control binded to click
                }
            }

            Scaleform.CallFunction("SET_BACKGROUND_COLOUR", (int)BackgroundColor.R, (int)BackgroundColor.G, (int)BackgroundColor.B, (int)BackgroundColor.A);
            Scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);

            needsUpdate = false;
        }

        public class InstructionalButtonsCollection : BaseCollection<IInstructionalButtonSlot>
        {
        }
    }

    public interface IInstructionalButtonSlot
    {
        /// <summary>
        /// Gets or sets the text displayed next to the button.
        /// <para>
        /// If this <see cref="IInstructionalButtonSlot"/> is contained in a <see cref="InstructionalButtons"/> scaleform, 
        /// <see cref="InstructionalButtons.Update"/> needs to be called to reflect the changes made.
        /// </para>
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the text displayed next to the button.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Predicate{T}"/> that defines the conditions for this <see cref="IInstructionalButtonSlot"/> to be visible.
        /// <para>
        /// If this <see cref="IInstructionalButtonSlot"/> is contained in a <see cref="InstructionalButtons"/> scaleform, 
        /// this predicate is only evaluated when <see cref="InstructionalButtons.Update"/> is called.
        /// </para>
        /// </summary>
        /// <value>
        /// A <see cref="Predicate{T}"/> delegate that defines the conditions for this <see cref="InstructionalButton"/> to be visible.
        /// </value>
        public Predicate<IInstructionalButtonSlot> CanBeDisplayed { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="GameControl"/> triggered when the button is clicked. If <c>null</c>, the button cannot be clicked.
        /// </summary>
        public GameControl? BindedControl { get; set; }

        /// <summary>
        /// Gets a <see cref="string"/> that represents the contents of this <see cref="IInstructionalButtonSlot"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the contents of this <see cref="IInstructionalButtonSlot"/>.</returns>
        public string GetButtonId();
    }

    public class InstructionalButton : IInstructionalButtonSlot
    {
        /// <inheritdoc/>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the contents of the button.
        /// <para>
        /// If this <see cref="InstructionalButton"/> is contained in a <see cref="InstructionalButtons"/> scaleform, 
        /// <see cref="InstructionalButtons.Update"/> needs to be called to reflect the changes made.
        /// </para>
        /// </summary>
        public InstructionalButtonId Button { get; set; }

        /// <inheritdoc/>
        public Predicate<IInstructionalButtonSlot> CanBeDisplayed { get; set; }
        
        /// <inheritdoc/>
        public GameControl? BindedControl { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButton"/> class.
        /// </summary>
        /// <param name="button">The button to be displayed.</param>
        /// <param name="text">The text displayed next to the button.</param>
        public InstructionalButton(InstructionalButtonId button, string text)
        {
            Text = text;
            Button = button;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButton"/> class. This overload sets <see cref="BindedControl"/> to <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The <see cref="GameControl"/> displayed inside the button, it changes depending on keybinds and whether the user is using the controller or the keyboard and mouse.</param>
        /// <param name="text">The text displayed next to the button.</param>
        public InstructionalButton(GameControl control, string text) : this((InstructionalButtonId)control, text) { BindedControl = control; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButton"/> class.
        /// </summary>
        /// <param name="buttonText">The text displayed inside the button, mainly for displaying custom keyboard bindings, like "I", or "O", or "F5".</param>
        /// <param name="text">The text displayed next to the button.</param>
        public InstructionalButton(string buttonText, string text) : this((InstructionalButtonId)buttonText, text) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButton"/> class.
        /// </summary>
        /// <param name="rawId">The raw identifier of the symbol to be displayed.</param>
        /// <param name="text">The text displayed next to the button.</param>
        public InstructionalButton(uint rawId, string text) : this((InstructionalButtonId)rawId, text) { }

        /// <inheritdoc/>
        public string GetButtonId() => Button.Id;

        public static string GetButtonId(GameControl control) => new InstructionalButtonId(control).Id;
        public static string GetButtonId(string keyString) => new InstructionalButtonId(keyString).Id;
        public static string GetButtonId(uint rawId) => new InstructionalButtonId(rawId).Id;
    }

    public class InstructionalButtonGroup : IInstructionalButtonSlot
    {
        private IList<InstructionalButtonId> buttons;

        /// <inheritdoc/>
        public string Text { get; set; }

        /// <inheritdoc/>
        public Predicate<IInstructionalButtonSlot> CanBeDisplayed { get; set; }

        /// <inheritdoc/>
        public GameControl? BindedControl { get; set; }

        /// <summary>
        /// Gets or sets the list containing the buttons of this group.
        /// <para>
        /// If this <see cref="InstructionalButtonGroup"/> is contained in a <see cref="InstructionalButtons"/> scaleform, 
        /// <see cref="InstructionalButtons.Update"/> needs to be called to reflect the changes made.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <c>value</c> is null.
        /// </exception>
        public IList<InstructionalButtonId> Buttons
        {
            get => buttons;
            set => buttons = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButtonGroup"/> class.
        /// </summary>
        /// <param name="buttons">The buttons to be displayed.</param>
        /// <param name="text">The text displayed next to the buttons.</param>
        public InstructionalButtonGroup(IEnumerable<InstructionalButtonId> buttons, string text)
        {
            Text = text;
            Buttons = new List<InstructionalButtonId>(buttons);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButtonGroup"/> class.
        /// </summary>
        /// <param name="controls">The <see cref="GameControl"/>s contained in the group, they change depending on keybinds and whether the user is using the controller or the keyboard and mouse.</param>
        /// <param name="text">The text displayed next to the buttons.</param>
        public InstructionalButtonGroup(IEnumerable<GameControl> controls, string text) : this(controls.Select(x => (InstructionalButtonId)x), text) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButtonGroup"/> class.
        /// </summary>
        /// <param name="buttonTexts">The text displayed inside the buttons in the group, mainly for displaying custom keyboard bindings, like "I", or "O", or "F5".</param>
        /// <param name="text">The text displayed next to the buttons.</param>
        public InstructionalButtonGroup(IEnumerable<string> buttonTexts, string text) : this(buttonTexts.Select(x => (InstructionalButtonId)x), text) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButtonGroup"/> class.
        /// </summary>
        /// <param name="rawIds">The raw identifiers of the symbols to be displayed in the group.</param>
        /// <param name="text">The text displayed next to the buttons.</param>
        public InstructionalButtonGroup(IEnumerable<uint> rawIds, string text) : this(rawIds.Select(x => (InstructionalButtonId)x), text) { }

        /// <inheritdoc/>
        public string GetButtonId() => GetButtonId(Buttons);

        public static string GetButtonId(IEnumerable<InstructionalButtonId> buttons)
            => buttons.Reverse().Aggregate(string.Empty, (acc, btn) => acc + (acc.Length == 0 ? string.Empty : "%") + btn.Id);
    }

    public readonly struct InstructionalButtonId
    {
        /// <summary>
        /// A blank button. Used when this struct is created with the default constructor or <c>default</c> value.
        /// </summary>
        private const string EmptyButtonId = "t_";

        // need to store the control because the string returned by GET_CONTROL_INSTRUCTIONAL_BUTTON may change
        // if the user switches between keyboard and controller
        private readonly GameControl? control;
        private readonly string id;

        /// <summary>
        /// Gets the <see cref="string"/> that represents the button contents.
        /// </summary>
        public string Id => id ?? (control.HasValue ? N.GetControlInstructionalButton(2, control.Value)  : EmptyButtonId);

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButtonId"/> structure.
        /// </summary>
        /// <param name="control">The <see cref="GameControl"/> displayed inside the button, it changes depending on keybinds and whether the user is using the controller or the keyboard and mouse.</param>
        public InstructionalButtonId(GameControl control) : this() => this.control = control;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButtonId"/> structure.
        /// </summary>
        /// <param name="str">The text displayed inside the button, mainly for displaying custom keyboard bindings, like "I", or "O", or "F5".</param>
        public InstructionalButtonId(string str) : this()
            => id = str.Length switch
            {
                int n when n <= 2 => "t_",
                int n when n <= 4 => "T_",
                _ => "w_"
            } + str;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButtonId"/> structure.
        /// </summary>
        /// <param name="rawId">The raw identifier of the symbol to be displayed.</param>
        public InstructionalButtonId(uint rawId) : this() => id = "b_" + rawId;

        public static implicit operator InstructionalButtonId(GameControl control) => new InstructionalButtonId(control);
        public static implicit operator InstructionalButtonId(string str) => new InstructionalButtonId(str);
        public static implicit operator InstructionalButtonId(uint rawId) => new InstructionalButtonId(rawId);
    }
}
