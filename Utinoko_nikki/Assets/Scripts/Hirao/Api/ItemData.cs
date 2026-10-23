using System;

namespace AquaGarden.Api
{
    [Serializable]
    public class ItemData
    {
        public int id;
        public string name;
        public int type;
    }

    [Serializable]
    public class ItemListResponse
    {
        public ItemData[] items;
    }
}