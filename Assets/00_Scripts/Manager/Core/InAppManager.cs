using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class InAppManager : IDetailedStoreListener
{
    public readonly string gem01 = "dia150";
    public readonly string gem02 = "dia330";
    public readonly string gem03 = "dia850";

    private IStoreController storeController;
    private IExtensionProvider storeExtensionProvider;

    public void Init()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(gem01, ProductType.Consumable, new IDs() { { gem01, GooglePlay.Name}});
        builder.AddProduct(gem02, ProductType.Consumable, new IDs() { { gem02, GooglePlay.Name}});
        builder.AddProduct(gem03, ProductType.Consumable, new IDs() { { gem03, GooglePlay.Name}});

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensionProvider = extensions;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("구매 실패");
    }

    public void Purchase(string productId)
    {
        Product product = GetProduct(productId);
        if (productId != null & product.availableToPurchase)
        {
            storeController.InitiatePurchase(product);
        }
        else 
        {
            Debug.Log("상품이 없거나 현재 구매가 불가능합니다.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        UI_LobbyScene lobbyScene = Managers.UI.SceneUI as UI_LobbyScene;
        switch (purchaseEvent.purchasedProduct.definition.id)
        {
            case "dia150":
                lobbyScene.ShopUI.UpdateMaterials(150);
                break;
            case "dia330":
                lobbyScene.ShopUI.UpdateMaterials(330);
                break;
            case "dia850":
                lobbyScene.ShopUI.UpdateMaterials(850);
                break;
        }
        

        return PurchaseProcessingResult.Complete;
    }

    public Product GetProduct(string productId)
    {
        return storeController.products.WithID(productId);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"{product.definition.id} OnPurchaseFailed");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"{error} OnPurchaseFailed");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log($"{error} OnPurchaseFailed" + message);
    }
}
