using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : IStoreListener {

	private IStoreController controller;
	private IExtensionProvider extensions;

	public IAPManager () {
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		builder.AddProduct("No_Ads", ProductType.NonConsumable);

		UnityPurchasing.Initialize (this, builder);
	}

	// Example method called when the user presses a 'buy' button
	// to start the purchase process.
	public void OnPurchaseClicked(string productId) {
		controller.InitiatePurchase(productId);
	}



	/// <summary>
	/// Called when Unity IAP is ready to make purchases.
	/// </summary>
	public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
	{
		this.controller = controller;
		this.extensions = extensions;

		extensions.GetExtension<IAppleExtensions> ().RestoreTransactions (result => {
			if (result) {
				// This does not mean anything was restored,
				// merely that the restoration process succeeded.
			} else {
				// Restoration failed.
			}
		});

	}

	/// <summary>
	/// Called when Unity IAP encounters an unrecoverable initialization error.
	///
	/// Note that this will not be called if Internet is unavailable; Unity IAP
	/// will attempt initialization until it becomes available.
	/// </summary>
	public void OnInitializeFailed (InitializationFailureReason error)
	{
	}

	/// <summary>
	/// Called when a purchase completes.
	///
	/// May be called at any time after OnInitialized().
	/// </summary>
	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
	{
		Debug.Log("Purchase Complete");

		return PurchaseProcessingResult.Complete;
	}

	/// <summary>
	/// Called when a purchase fails.
	/// </summary>
	public void OnPurchaseFailed (Product i, PurchaseFailureReason p)
	{
		Debug.Log("Purchase FAILED");
	}



}
