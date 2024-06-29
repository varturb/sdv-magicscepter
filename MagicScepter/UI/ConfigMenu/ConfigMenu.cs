using System;
using System.Collections.Generic;
using System.Linq;
using MagicScepter.Constants;
using MagicScepter.Handlers;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class ConfigMenu : IClickableMenu
  {
    public List<TeleportScroll> teleportScrolls;
    private readonly Texture2D spritesheetTexture;
    private readonly Texture2D scrollsTexture;
    private readonly List<VisibilityButton> visibilityButtons = new();
    private readonly List<RenameButton> renameButtons = new();
    private readonly List<MoveUpButton> moveUpButtons = new();
    private readonly List<MoveDownButton> moveDownButtons = new();
    private readonly List<KeybindButton> keybindButtons = new();
    private readonly List<ClickableComponent> rows = new();
    private ClickableTextureComponent scrollUpButton;
    private ClickableTextureComponent scrollDownButton;
    private ClickableTextureComponent scrollBar;
    private ClickableTextureComponent configPageButon;
    private ClickableTextureComponent settingsPageButon;
    private Rectangle scrollBarRunner;
    private int topRowIndex = 0;
    private const int configPageButtonID = 100;
    private const int settingsPageButtonID = 101;
    private const int moveUpButtonOffset = 200;
    private const int moveDownButtonOffset = 300;
    private const int visibilityButtonOffset = 400;
    private const int renameButtonOffset = 500;
    private const int keybindButtonOffset = 600;
    private const int pageSize = 6;
    private string hoverText = string.Empty;
    private bool scrolling;

    public ConfigMenu()
    {
      width = 832;
      height = 576;

      teleportScrolls = ScrollHandler.GetTeleportScrolls();
      spritesheetTexture = FileHelper.GetSpritesheetTexture();
      scrollsTexture = FileHelper.GetScrollsTexture();

      topRowIndex = 0;
      ResetLayout();

      if (Game1.options.SnappyMenus)
      {
        currentlySnappedComponent = renameButtons.First().ClickableComponent;
        base.snapCursorToCurrentSnappedComponent();
      }
    }

    public void ResetLayout()
    {
      SetPosition();
      CreateComponents();
      SetScrollBarToCurrentIndex();
      populateClickableComponentList();
    }

    public void SetPosition()
    {
      var topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
      xPositionOnScreen = (int)topLeft.X;
      yPositionOnScreen = (int)topLeft.Y + 32;
    }

    public void RefreshTeleportScrolls()
    {
      teleportScrolls = ScrollHandler.GetTeleportScrolls();
      ResetLayout();
    }

    private void CreateComponents()
    {
      moveUpButtons.Clear();
      moveDownButtons.Clear();
      visibilityButtons.Clear();
      renameButtons.Clear();
      rows.Clear();

      for (int i = 0; i < teleportScrolls.Count; i++)
      {
        var row = new ClickableComponent(
          new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + 16 + (i - topRowIndex) * ((height - 32) / pageSize), width - 32, (height - 32) / pageSize + 4),
          string.Concat(i)
        );

        var moveUpButton = new MoveUpButton(i, teleportScrolls, this, skip: i == 0);
        var moveDownButton = new MoveDownButton(i, teleportScrolls, this, skip: i == teleportScrolls.Count - 1);
        var visibilityButton = new VisibilityButton(teleportScrolls[i], this, skip: IsFarmScroll(i));
        var renameButton = new RenameButton(teleportScrolls[i]);
        var keybindButton = new KeybindButton(teleportScrolls[i]);

        moveUpButtons.Add(moveUpButton);
        moveDownButtons.Add(moveDownButton);
        visibilityButtons.Add(visibilityButton);
        renameButtons.Add(renameButton);
        keybindButtons.Add(keybindButton);
        rows.Add(row);
      }

      scrollUpButton = new ClickableTextureComponent(
        new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen, 44, 48),
        spritesheetTexture,
        new Rectangle(64, 18, 12, 12),
        4f
        );
      scrollDownButton = new ClickableTextureComponent(
        new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48),
        spritesheetTexture,
        new Rectangle(64, 31, 12, 12),
        4f
      );
      scrollBar = new ClickableTextureComponent(
        new Rectangle(scrollUpButton.bounds.X + 12, scrollUpButton.bounds.Y + scrollUpButton.bounds.Height + 4, 24, 40),
        spritesheetTexture,
        new Rectangle(64, 43, 6, 10),
        4f
      );
      scrollBarRunner = new Rectangle(
        scrollBar.bounds.X,
        scrollUpButton.bounds.Y + scrollUpButton.bounds.Height + 4,
        scrollBar.bounds.Width,
        height - 64 - scrollUpButton.bounds.Height - 8
      );
      configPageButon = new ClickableTextureComponent(
        new Rectangle(xPositionOnScreen - 64 + 4, yPositionOnScreen + 24, 64, 64),
        spritesheetTexture,
        new Rectangle(66, 64, 32, 32),
        2f
      )
      {
        myID = configPageButtonID,
        upNeighborID = -1,
        downNeighborID = settingsPageButtonID,
        rightNeighborID = moveDownButtonOffset + topRowIndex,
        leftNeighborID = -1,
        fullyImmutable = true
      };
      settingsPageButon = new ClickableTextureComponent(
        new Rectangle(xPositionOnScreen - 64, yPositionOnScreen + 24 + 64, 64, 64),
        spritesheetTexture,
        new Rectangle(34, 64, 32, 32),
        2f
      )
      {
        myID = settingsPageButtonID,
        upNeighborID = configPageButtonID,
        downNeighborID = -1,
        rightNeighborID = moveDownButtonOffset + topRowIndex,
        leftNeighborID = -1,
        fullyImmutable = true
      };

      initializeUpperRightCloseButton();
      upperRightCloseButton.myID = upperRightCloseButton_ID;
      upperRightCloseButton.leftNeighborID = keybindButtonOffset + topRowIndex;
      upperRightCloseButton.downNeighborID = keybindButtonOffset + topRowIndex;
      upperRightCloseButton.fullyImmutable = true;

      SetupIDs();
    }

    private void SetupIDs()
    {
      for (int i = 0; i < teleportScrolls.Count; i++)
      {
        moveUpButtons[i].SetupIDs(
          ID: i > 0 ? i + moveUpButtonOffset : -500,
          upID: i > 0 ? i - 1 + moveDownButtonOffset : -1,
          downID: i + moveDownButtonOffset,
          leftID: settingsPageButtonID,
          rightID: IsFarmScroll(i) ? i + renameButtonOffset : i + visibilityButtonOffset
        );

        moveDownButtons[i].SetupIDs(
          ID: i < teleportScrolls.Count - 1 ? i + moveDownButtonOffset : -500,
          upID: i + moveUpButtonOffset,
          downID: i + 1 + moveUpButtonOffset,
          leftID: settingsPageButtonID,
          rightID: IsFarmScroll(i) ? i + renameButtonOffset : i + visibilityButtonOffset
        );

        visibilityButtons[i].SetupIDs(
          ID: i + visibilityButtonOffset,
          upID: i > 0 ? i - 1 + (IsFarmScroll(i - 1) ? renameButtonOffset : visibilityButtonOffset) : -1,
          downID: i + 1 + (IsFarmScroll(i + 1) ? renameButtonOffset : visibilityButtonOffset),
          leftID: i < teleportScrolls.Count - 1 ? i + moveDownButtonOffset : i + moveUpButtonOffset,
          rightID: i + renameButtonOffset
        );

        renameButtons[i].SetupIDs(
          ID: i + renameButtonOffset,
          upID: i > 0 ? i - 1 + renameButtonOffset : -500,
          downID: i + 1 + renameButtonOffset,
          leftID: i + (IsFarmScroll(i) ? i < teleportScrolls.Count - 1 ? i + moveDownButtonOffset : i + moveUpButtonOffset : visibilityButtonOffset),
          rightID: i + keybindButtonOffset
        );

        keybindButtons[i].SetupIDs(
          ID: i + keybindButtonOffset,
          upID: i > 0 ? i - 1 + keybindButtonOffset : upperRightCloseButton_ID,
          downID: i + 1 + keybindButtonOffset,
          leftID: i + renameButtonOffset,
          rightID: i > 0 ? -1 : upperRightCloseButton_ID
        );
      }
    }

    private bool IsFarmScroll(int index)
    {
      return index > -1 && teleportScrolls.Count > index && teleportScrolls[index].ActionDoWhen.Do.Type == ActionDoType.Farm;
    }

    private void SetScrollBarToCurrentIndex()
    {
      if (rows.Count > 0)
      {
        scrollBar.bounds.Y = scrollBarRunner.Height / Math.Max(1, rows.Count - pageSize + 1) * topRowIndex + scrollUpButton.bounds.Bottom + 4;
        if (topRowIndex == rows.Count - pageSize)
        {
          scrollBar.bounds.Y = scrollDownButton.bounds.Y - scrollBar.bounds.Height - 4;
        }
      }

      UpdateRowsPositions();
    }

    private void UpdateRowsPositions()
    {
      for (int i = 0; i < rows.Count; i++)
      {
        var row = rows[i];
        var y = yPositionOnScreen + 16 + (i - topRowIndex) * ((height - 32) / pageSize);
        row.bounds.Y = y;

        moveUpButtons[i].UpdatePosition(row.bounds.Left + 12, row.bounds.Y + 12);
        moveDownButtons[i].UpdatePosition(row.bounds.Left + 12, row.bounds.Y + row.bounds.Height / 2);
        visibilityButtons[i].UpdatePosition(row.bounds.Right - row.bounds.Width / 12 * 3 + 12, row.bounds.Y + row.bounds.Height / 2 - 18);
        renameButtons[i].UpdatePosition(row.bounds.Right - row.bounds.Width / 12 * 2, row.bounds.Y + row.bounds.Height / 2 - 20);
        keybindButtons[i].UpdatePosition(row.bounds.Right - row.bounds.Width / 12 - 12, row.bounds.Y + row.bounds.Height / 2 - 24);
      }

      configPageButon.rightNeighborID = topRowIndex + moveDownButtonOffset;
      settingsPageButon.rightNeighborID = topRowIndex + moveDownButtonOffset;
    }

    private void UpArrowPressed()
    {
      topRowIndex--;
      scrollUpButton.scale = 3.5f;
      SetScrollBarToCurrentIndex();
    }

    private void DownArrowPressed()
    {
      topRowIndex++;
      scrollDownButton.scale = 3.5f;
      SetScrollBarToCurrentIndex();
    }

    private void SettingsPageButtonPressed()
    {
      exitThisMenu();
      Game1.activeClickableMenu = new SettingsMenu();
    }

    private void ConstrainSelectionToVisibleSlots()
    {
      if (allClickableComponents.Contains(currentlySnappedComponent))
      {
        int index = allClickableComponents.IndexOf(currentlySnappedComponent) % teleportScrolls.Count;
        if (index < topRowIndex)
        {
          index = topRowIndex;
        }
        else if (index >= topRowIndex + pageSize)
        {
          index = topRowIndex + pageSize - 1;
        }

        currentlySnappedComponent = index > 0 ? visibilityButtons[index].ClickableComponent : renameButtons[index].ClickableComponent;
        if (Game1.options.SnappyMenus)
        {
          snapCursorToCurrentSnappedComponent();
        }
      }
    }

    public override void applyMovementKey(int direction)
    {
      base.applyMovementKey(direction);
      if (allClickableComponents.Contains(currentlySnappedComponent))
      {
        int index = allClickableComponents.IndexOf(currentlySnappedComponent) % rows.Count;
        if (index < topRowIndex)
        {
          topRowIndex = index;
        }
        else if (index >= topRowIndex + pageSize)
        {
          topRowIndex = index - pageSize + 1;
        }

        SetScrollBarToCurrentIndex();
        UpdateRowsPositions();

        if (Game1.options.SnappyMenus)
        {
          snapCursorToCurrentSnappedComponent();
        }
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (settingsPageButon.containsPoint(x, y))
      {
        SettingsPageButtonPressed();
        return;
      }

      if (scrollUpButton.containsPoint(x, y) && topRowIndex > 0)
      {
        UpArrowPressed();
        Game1.playSound("shwip");
        return;
      }

      if (scrollDownButton.containsPoint(x, y) && topRowIndex < rows.Count - pageSize)
      {
        DownArrowPressed();
        Game1.playSound("shwip");
        return;
      }

      if (scrollBar.containsPoint(x, y))
      {
        scrolling = true;
        return;
      }

      for (int i = topRowIndex; i < topRowIndex + pageSize && i < rows.Count; i++)
      {
        moveUpButtons[i].receiveLeftClick(x, y, playSound);
        moveDownButtons[i].receiveLeftClick(x, y, playSound);
        visibilityButtons[i].receiveLeftClick(x, y, playSound);
        renameButtons[i].receiveLeftClick(x, y, playSound);
        keybindButtons[i].receiveLeftClick(x, y, playSound);
      }

      topRowIndex = Math.Max(0, Math.Min(rows.Count - pageSize, topRowIndex));

      base.receiveLeftClick(x, y, playSound);
    }

    public override void releaseLeftClick(int x, int y)
    {
      base.releaseLeftClick(x, y);
      scrolling = false;
    }

    public override void leftClickHeld(int x, int y)
    {
      base.leftClickHeld(x, y);
      if (scrolling)
      {
        int y2 = scrollBar.bounds.Y;
        scrollBar.bounds.Y = Math.Min(yPositionOnScreen + height - 64 - 12 - scrollBar.bounds.Height, Math.Max(y, yPositionOnScreen + scrollUpButton.bounds.Height + 20));
        float num = (float)(y - scrollBarRunner.Y) / (float)scrollBarRunner.Height;
        topRowIndex = Math.Min(Math.Max(0, rows.Count - pageSize), Math.Max(0, (int)((float)rows.Count * num)));
        SetScrollBarToCurrentIndex();
        if (y2 != scrollBar.bounds.Y)
        {
          Game1.playSound("shiny4");
        }
      }
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);

      if (rows.Count > 0)
      {
        scrollUpButton.tryHover(x, y);
        scrollDownButton.tryHover(x, y);
      }

      for (int i = topRowIndex; i < topRowIndex + pageSize && i < rows.Count; i++)
      {
        moveUpButtons[i].performHoverAction(x, y);
        moveDownButtons[i].performHoverAction(x, y);
        visibilityButtons[i].performHoverAction(x, y);
        renameButtons[i].performHoverAction(x, y);
        keybindButtons[i].performHoverAction(x, y);
      }
    }

    public override void receiveScrollWheelAction(int direction)
    {
      base.receiveScrollWheelAction(direction);
      if (direction > 0 && topRowIndex > 0)
      {
        UpArrowPressed();
        ConstrainSelectionToVisibleSlots();
        Game1.playSound("shiny4");
      }
      else if (direction < 0 && topRowIndex < Math.Max(0, rows.Count - pageSize))
      {
        DownArrowPressed();
        ConstrainSelectionToVisibleSlots();
        Game1.playSound("shiny4");
      }
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.AddRange(moveUpButtons.Select(b => b.ClickableComponent));
      allClickableComponents.AddRange(moveDownButtons.Select(b => b.ClickableComponent));
      allClickableComponents.AddRange(visibilityButtons.Select(b => b.ClickableComponent));
      allClickableComponents.AddRange(renameButtons.Select(b => b.ClickableComponent));
      allClickableComponents.AddRange(keybindButtons.Select(b => b.ClickableComponent));
      allClickableComponents.Add(upperRightCloseButton);
      allClickableComponents.Add(configPageButon);
      allClickableComponents.Add(settingsPageButon);
    }

    public override void snapToDefaultClickableComponent()
    {
      if (topRowIndex < teleportScrolls.Count)
      {
        currentlySnappedComponent = topRowIndex > 0 ? visibilityButtons[topRowIndex].ClickableComponent : renameButtons[topRowIndex].ClickableComponent;
      }

      base.snapCursorToCurrentSnappedComponent();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      ResetLayout();
    }

    public override void draw(SpriteBatch b)
    {
      var color = Color.White;

      // draw faded background
      GameHelper.DrawFadedBackground(b, 0.2f);
      // draw menu title
      GameHelper.DrawTextInScroll(b, I18n.ConfigurationMenu_Title(), xPositionOnScreen + width / 2, yPositionOnScreen - 64);

      // draw menu box
      drawTextureBox(
        b,
        spritesheetTexture,
        new Rectangle(98, 64, 18, 18),
        xPositionOnScreen,
        yPositionOnScreen,
        width,
        height,
        Color.White,
        4f
      );

      hoverText = string.Empty;

      for (int i = topRowIndex; i < topRowIndex + pageSize && i < rows.Count; i++)
      {
        var row = rows[i];
        // draw row box
        drawTextureBox(
          b,
          spritesheetTexture,
          new Rectangle(98, 82, 15, 15),
          row.bounds.X,
          row.bounds.Y,
          row.bounds.Width,
          row.bounds.Height,
          color,
          4f,
          drawShadow: false
        );

        // draw scroll
        drawTextureBox(
          b,
          spritesheetTexture,
          new Rectangle(0, 0, 64, 64),
          row.bounds.X + 48,
          row.bounds.Y + 16,
          64,
          64,
          color * 0.9f,
          1f,
          false
        );
        drawTextureBox(
          b,
          scrollsTexture,
          teleportScrolls[i].SpirteSource,
          row.bounds.X + 48,
          row.bounds.Y + 16,
          64,
          64,
          color * 0.9f,
          1f,
          false
        );

        // draw name
        SpriteText.drawString(
          b,
          teleportScrolls[i].Text,
          row.bounds.X + 128,
          row.bounds.Y + 24
        );
      }

      // draw page buttons
      var mouseX = Game1.getMouseX();
      var mouseY = Game1.getMouseY();

      if (configPageButon.containsPoint(mouseX, mouseY)) hoverText = I18n.ConfigurationMenu_Title();
      configPageButon.draw(b);

      if (settingsPageButon.containsPoint(mouseX, mouseY)) hoverText = I18n.SettingsMenu_Title();
      color = hoverText == I18n.SettingsMenu_Title() ? Color.White : Color.White * 0.8f;
      settingsPageButon.draw(b, color, GameHelper.CalculateDepth(settingsPageButon.bounds.Y));

      // draw scrollbar
      if (rows.Count > pageSize)
      {
        scrollUpButton.draw(b);
        scrollDownButton.draw(b);
        drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f);
        scrollBar.draw(b);
      }

      // draw buttons
      for (int i = topRowIndex; i < topRowIndex + pageSize && i < rows.Count; i++)
      {
        moveUpButtons[i].draw(b);
        moveDownButtons[i].draw(b);
        visibilityButtons[i].draw(b);
        renameButtons[i].draw(b);
        keybindButtons[i].draw(b);

        if (moveUpButtons[i].Hovered) hoverText = moveUpButtons[i].HoverText;
        if (moveDownButtons[i].Hovered) hoverText = moveDownButtons[i].HoverText;
        if (visibilityButtons[i].Hovered) hoverText = visibilityButtons[i].HoverText;
        if (renameButtons[i].Hovered) hoverText = renameButtons[i].HoverText;
        if (keybindButtons[i].Hovered) hoverText = keybindButtons[i].HoverText;
      }

      // draw hover text
      if (hoverText.IsNotEmpty()) drawHoverText(b, hoverText, Game1.smallFont);

      base.draw(b);
      drawMouse(b);
    }
  }
}