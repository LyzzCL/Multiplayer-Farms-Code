namespace MPF_Code.Config
{
    public sealed class ModConfig
    {
        public bool Free_Cabins { get; set; } = true;
        public bool Starter_Cabins { get; set; } = true;
        public bool Starter_Items { get; set; } = true;
        public bool Starter_Builds { get; set; } = true;

        public bool Return_Scepter { get; set; } = true;
        public bool Farm_Totem { get; set; } = true;
        public bool Lightning_Rod { get; set; } = true;
        public bool Mini_Obelisks { get; set; } = true;
        public bool Fix_Trees { get; set; } = true;
        public bool Fix_Events { get; set; } = true;
        public bool Fix_Festivals { get; set; } = true;

        public bool F1_Enabled { get; set; } = true;
        public string F1_Name { get; set; } = "Wilderness";
        public string F1_Coords_1 { get; set; } = "0, 12";
        public string F1_Coords_1_Mode { get; set; } = "HorizontalLeft";
        public string F1_Coords_2 { get; set; } = "0, 0";
        public string F1_Coords_2_Mode { get; set; } = "VerticalUp";
        public string F1_Type { get; set; } = "Wilderness";

        public bool F2_Enabled { get; set; } = true;
        public string F2_Name { get; set; } = "Forest";
        public string F2_Coords_1 { get; set; } = "79, 46";
        public string F2_Coords_1_Mode { get; set; } = "HorizontalRight";
        public string F2_Coords_2 { get; set; } = "79, 15";
        public string F2_Coords_2_Mode { get; set; } = "HorizontalRight";
        public string F2_Type { get; set; } = "Forest";

        public bool F3_Enabled { get; set; } = true;
        public string F3_Name { get; set; } = "Beach";
        public string F3_Coords_1 { get; set; } = "7, 0";
        public string F3_Coords_1_Mode { get; set; } = "VerticalUp";
        public string F3_Coords_2 { get; set; } = "0, 19";
        public string F3_Coords_2_Mode { get; set; } = "HorizontalLeft";
        public string F3_Type { get; set; } = "Beach";

        public bool F4_Enabled { get; set; } = true;
        public string F4_Name { get; set; } = "Riverland";
        public string F4_Coords_1 { get; set; } = "40, 0";
        public string F4_Coords_1_Mode { get; set; } = "VerticalUp";
        public string F4_Coords_2 { get; set; } = "40, 64";
        public string F4_Coords_2_Mode { get; set; } = "VerticalDown";
        public string F4_Type { get; set; } = "Riverland";

        public bool F5_Enabled { get; set; } = true;
        public string F5_Name { get; set; } = "Meadowlands";
        public string F5_Coords_1 { get; set; } = "99, 21";
        public string F5_Coords_1_Mode { get; set; } = "HorizontalRight";
        public string F5_Coords_2 { get; set; } = "0, 0";
        public string F5_Coords_2_Mode { get; set; } = "HorizontalLeft";
        public string F5_Type { get; set; } = "Meadowlands";

        public bool F6_Enabled { get; set; } = true;
        public string F6_Name { get; set; } = "Hilltop";
        public string F6_Coords_1 { get; set; } = "40, 64";
        public string F6_Coords_1_Mode { get; set; } = "VerticalDown";
        public string F6_Coords_2 { get; set; } = "0, 10";
        public string F6_Coords_2_Mode { get; set; } = "HorizontalLeft";
        public string F6_Type { get; set; } = "Hilltop";

        public bool F7_Enabled { get; set; } = true;
        public string F7_Name { get; set; } = "Four Corners";
        public string F7_Coords_1 { get; set; } = "40, 64";
        public string F7_Coords_1_Mode { get; set; } = "VerticalDown";
        public string F7_Coords_2 { get; set; } = "79, 17";
        public string F7_Coords_2_Mode { get; set; } = "HorizontalRight";
        public string F7_Type { get; set; } = "4Corners";
    }
}
