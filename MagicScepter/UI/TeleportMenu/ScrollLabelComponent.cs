using System;
using System.Collections.Generic;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class ScrollLabelComponent : ClickableComponent
  {
    public List<ClickableComponent> ClickableComponents => new() { passByComponent, configButton };
    public static int ComponentID => passByID;

    private readonly ClickableComponent passByComponent;
    private readonly TeleportMenuConfigButton configButton;
    private const int passByID = 500;
    private const int configButtonID = 501;
    private const string textOffset = "  ";
    private readonly static string defaultLabel = I18n.TeleportMenu_Title();
    private readonly bool previewMode;

    public ScrollLabelComponent(Action<SpriteBatch, string> drawAction, bool previewMode = false)
      : base(new Rectangle(0, 0, 0, 36), defaultLabel)
    {
      this.previewMode = previewMode;
      configButton = new TeleportMenuConfigButton(0, 0, drawAction);
      passByComponent = new ClickableComponent(default, string.Empty);
    }

    public void SetupIDs(int upID = -1, int downID = -1)
    {
      passByComponent.myID = passByID;
      passByComponent.upNeighborID = upID;
      passByComponent.downNeighborID = downID;
      passByComponent.rightNeighborID = configButtonID;
      passByComponent.fullyImmutable = true;

      configButton.myID = configButtonID;
      configButton.upNeighborID = upID;
      configButton.downNeighborID = downID;
      configButton.leftNeighborID = passByID;
      passByComponent.fullyImmutable = true;
    }


    public void SetDefaultLabel()
    {
      label = defaultLabel;
    }

    public void SetLabel(string label)
    {
      this.label = label;
    }

    public void UpdatePosition(int x, int y)
    {
      bounds.X = x;
      bounds.Y = y;
      bounds.Width = SpriteText.getWidthOfString(label.DefaultIfEmpty(defaultLabel));

      if (Context.IsMainPlayer)
      {
        passByComponent.bounds.X = x - SpriteText.getWidthOfString(defaultLabel) / 2;
        passByComponent.bounds.Y = y + bounds.Height / 2;
        passByComponent.bounds.Width = SpriteText.getWidthOfString(defaultLabel);
        passByComponent.bounds.Height = bounds.Height;
        passByComponent.visible = visible;

        configButton.bounds.X = x + bounds.Width / 2;
        configButton.bounds.Y = y + 8;
        configButton.visible = visible;
      }
      else
      {
        passByComponent.visible = false;
        configButton.visible = false;
      }
    }

    public void OnClick(int x, int y)
    {
      if (visible)
      {
        configButton.OnClick(x, y);
      }
    }

    public void PerformHoverAction(int x, int y)
    {
      configButton.tryHover(x, y);
    }

    public void Draw(SpriteBatch b)
    {
      if (!visible)
      {
        return;
      }

      if (Context.IsMainPlayer && !previewMode)
      {
        SpriteText.drawStringWithScrollCenteredAt(b, label + textOffset, bounds.X, bounds.Y);
        configButton.draw(b);
      }
      else
      {
        SpriteText.drawStringWithScrollCenteredAt(b, label, bounds.X, bounds.Y);
      }
    }
  }
}