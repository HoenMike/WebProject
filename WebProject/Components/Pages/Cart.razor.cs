using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor;
using WebProject.Components.Dialogs;
using WebProject.Models;

namespace WebProject.Components.Pages
{
  public partial class Cart
  {
    private List<CartItem> cartItems = new();
    private decimal totalPrice = 0;
    private decimal selectedTotalPrice = 0;
    private int selectedItemsCount = 0;
    private bool isRendered = false;
    private bool isLoading = true;
    private bool selectAll;

    protected override async Task OnInitializedAsync()
    {
      await LoadCart();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        isRendered = true;
        await LoadCart();
      }
    }

    private async Task LoadCart()
    {
      isLoading = true;
      cartItems = await CartService.GetCartItemsAsync();
      totalPrice = cartItems.Sum(item => item.Price * item.Quantity);

      // Reset selected items state
      selectAll = false;
      foreach (var item in cartItems)
      {
        item.IsSelected = false;
      }

      UpdateSelectedTotalPrice();
      isLoading = false;
      StateHasChanged();
    }

    private async Task DecreaseQuantity(CartItem item)
    {
      if (item.Quantity > 1)
      {
        item.Quantity--;
        var product = await CartService.GetProductByIdAsync(item.ItemId);
        if (product != null)
        {
          product.StockQuantity++;
          await CartService.UpdateQuantityAsync(item.ItemId, item.Quantity);
          await CartService.UpdateProductStockAsync(product);
          await LoadCart();
        }
      }
    }

    private async Task IncreaseQuantity(CartItem item)
    {
      var product = await CartService.GetProductByIdAsync(item.ItemId);
      if (product != null && item.Quantity < product.StockQuantity)
      {
        item.Quantity++;
        product.StockQuantity--;
        await CartService.UpdateQuantityAsync(item.ItemId, item.Quantity);
        await CartService.UpdateProductStockAsync(product);
        await LoadCart();
      }
      else
      {
        Snackbar.Add("Not enough stock available", Severity.Warning);
      }
    }

    private async Task RemoveFromCart(int itemId)
    {
      await CartService.RemoveFromCartAsync(itemId);
      await LoadCart();
      Snackbar.Add("Item removed from cart", Severity.Info);
    }
    private async Task ProceedToCheckout()
    {
      if (cartItems.Count == 0)
      {
        Snackbar.Add("Your cart is empty", Severity.Warning);
        return;
      }

      var selectedItems = cartItems.Where(item => item.IsSelected).ToList();
      if (selectedItems.Count == 0)
      {
        Snackbar.Add("Please select items to checkout", Severity.Warning);
        return;
      }

      var parameters = new DialogParameters();
      var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };

      var dialog = DialogService.Show<CheckoutDialog>("Checkout", parameters, options);
      var result = await dialog.Result;

      if (result != null && !result.Canceled)
      {
        try
        {
          await CartService.CheckoutAsync(selectedItems);
          Snackbar.Add("Checkout successful", Severity.Success);
          await LoadCart();
        }
        catch (Exception ex)
        {
          Snackbar.Add($"Checkout failed: {ex.Message}", Severity.Error);
        }
      }
    }
    private void SelectAllChanged(bool value)
    {
      selectAll = value;
      foreach (var item in cartItems)
      {
        item.IsSelected = value;
      }
      UpdateSelectedTotalPrice();
      StateHasChanged();
    }

    private void OnItemSelectedChanged(CartItem item, bool value)
    {
      item.IsSelected = value;
      selectAll = cartItems.All(x => x.IsSelected);
      UpdateSelectedTotalPrice();
    }

    private void UpdateSelectedTotalPrice()
    {
      var selectedItems = cartItems.Where(item => item.IsSelected).ToList();
      selectedTotalPrice = selectedItems.Sum(item => item.Price * item.Quantity);
      selectedItemsCount = selectedItems.Count;
    }
  }
}