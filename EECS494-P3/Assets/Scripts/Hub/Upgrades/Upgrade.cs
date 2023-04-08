namespace Hub.Upgrades {
    public class Upgrade : IsBuyable {
        // Use start in child classes to deal with thisData
        protected new virtual void Start() {
            if (thisData != null) {
                descriptionText.text = thisData.description;
                cost = thisData.cost;
            }

            base.Start();
        }
    }
}