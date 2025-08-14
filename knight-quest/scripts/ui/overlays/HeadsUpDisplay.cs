using System.Collections.Generic;
using System.Linq;
using Game.Autoloads;
using Game.Data;
using Godot;
using GodotUtilities;
using Game.Utils;

namespace Game.UI;

[Scene]
public partial class HeadsUpDisplay : MarginContainer
{
    [Node] private Marker2D playerPosition;
    [Node] private Marker2D enemyPosition;
    [Node] private RichTextLabel questionLabel;
    [Node] private Button firstAnswerButton;
    [Node] private GridContainer itemContainer;

    private List<Slot> slots;

    [Signal]
    public delegate void AnswerSelectedEventHandler(int index);

    public Vector2 PlayerGlobalPosition => playerPosition.GetGlobalPosition();
    public Vector2 EnemyGlobalPosition => enemyPosition.GetGlobalPosition();

    public override void _Notification(int what)
    {
        if (what != NotificationSceneInstantiated) return;
        WireNodes();
    }

    public override void _Ready()
    {
        slots = itemContainer.GetChildrenOfType<Slot>().ToList();
        foreach (var slot in slots)
        {   
            slot.Pressed += SelectSlot;
        }
        PopulateSlots();

        InventoryManager.Instance.Updated += OnInventoryUpdate;

        var answerButtonGroup = firstAnswerButton.ButtonGroup;
        answerButtonGroup.Pressed += OnAnswerButtonPressed;
        QuestionManager.Instance.QuestionRequested += OnQuestionRequested;
        
        
    }

    public override void _ExitTree()
    {
        QuestionManager.Instance.QuestionRequested -= OnQuestionRequested;
        InventoryManager.Instance.Updated -= OnInventoryUpdate;
    }

    // ===============================
    // QUESTION SYSTEM
    // ===============================
    private void Reset()
    {
        questionLabel.Text = string.Empty;

        var answerButtons = firstAnswerButton.ButtonGroup.GetButtons();
        foreach (var button in answerButtons)
        {
            if (button is not Button answerButton) continue;
            answerButton.Text = string.Empty;
            answerButton.Visible = false;
            answerButton.ButtonPressed = false;
            answerButton.Disabled = false;
        }
    }

    public void ToggleAnswerButtons(bool disabled = false)
    {
        var answerButtons = firstAnswerButton.ButtonGroup.GetButtons();
        foreach (var button in answerButtons)
        {
            if (button is not Button answerButton) continue;
            if (answerButton.ButtonPressed)
            {
                answerButton.ButtonPressed = false;
                continue;
            }

            answerButton.Disabled = disabled;
        }
    }

    private void OnAnswerButtonPressed(BaseButton button)
    {
        var answerIndex = button.GetMeta("answer_index").As<int>();
        Logger.Debug($"Answer selected: {answerIndex}");
        EmitSignalAnswerSelected(answerIndex);
        ToggleAnswerButtons(true);
    }

    private void OnQuestionRequested(Question question)
    {
        Reset();
        questionLabel.Text = question.QuestionText;

        var answerButtons = firstAnswerButton.ButtonGroup.GetButtons();
        for (var i = 0; i < answerButtons.Count; i++)
        {
            if (answerButtons[i] is not Button answerButton) continue;
            if (i < question.Answers.Length)
            {
                answerButton.Text = question.Answers[i];
                answerButton.Visible = true;
            }
            else
            {
                answerButton.Visible = false;
            }
        }
    }

    // ===============================
    // INVENTORY QUICK-USE
    // ===============================
    private void SelectSlot(Slot slot)
    {
        var selectedSlot = slots.FirstOrDefault(s => s.Selected);
        if (selectedSlot != null)
            selectedSlot.Selected = false;

        slot.Selected = true;
        UpdateSelectedItem(slot.Item);

        if (slot.Item is Consumable consumable)
            Logger.Info($"clicked {consumable.Name}");
    }

    private void UpdateSelectedItem(Item item)
    {
        if (item == null)
        {
            Logger.Debug("No item selected.");
            return;
        }

        Logger.Debug($"Selected item: {item.Name} x");
    }

    private void PopulateSlots()
    {
        var items = ItemRegistry.Resources.Values
            .OfType<Consumable>()
            .Where(c => c.Owned)
            .Cast<Item>()
            .ToList();

        Logger.Info($"Populating slots: {items.Count} items.");
        for (int i = 0; i < slots.Count && i < items.Count; i++)
        {
            var slot = slots[i];
            var item = items[i];
            Logger.Debug($"Slot {i}: Item={item.Name}, Owned={item is Consumable c && c.Owned}, Icon={(item.Icon != null ? "Yes" : "No")}");
            slot.Item = item;
            slot.icon.Texture = item.Icon;
        }
    }


    private void OnInventoryUpdate(Item item, int quantity)
    {
        PopulateSlots();
    }
}