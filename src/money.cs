using UnityEngine;

namespace HowToFishTrainer
{
    public static class MoneyTab
    {
        private static string _amount = "1000";

        public static void Draw()
        {
            GUILayout.Label("BALANCE", Theme.Header);
            GUILayout.Label("$" + MoneyManager.Money.ToString("N0"), Theme.Big);

            GUILayout.Label("AMOUNT", Theme.Header);
            _amount = GUILayout.TextField(_amount, 12, Theme.TextField);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", Theme.Button)) Add(Parse());
            if (GUILayout.Button("Remove", Theme.Button)) Remove(Parse());
            GUILayout.EndHorizontal();

            GUILayout.Label("QUICK", Theme.Header);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+1K", Theme.Button)) Add(1000);
            if (GUILayout.Button("+10K", Theme.Button)) Add(10000);
            if (GUILayout.Button("+100K", Theme.Button)) Add(100000);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Remove ALL money", Theme.Button))
                Remove(MoneyManager.Money);

            GUILayout.Label("Only works while you are the host - the game checks "
                          + "IsServerInitialized before touching the balance.", Theme.Small);
        }

        private static int Parse()
        {
            long v;
            if (!long.TryParse(_amount, out v)) return 0;
            return (int)System.Math.Min(v, int.MaxValue);
        }

        private static void Add(int amount)
        {
            if (amount > 0) MoneyManager.AddMoney(amount, Player.LocalPlayer);
        }

        private static void Remove(int amount)
        {
            if (amount > 0) MoneyManager.RemoveMoney(amount, Player.LocalPlayer);
        }
    }
}
