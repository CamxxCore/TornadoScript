using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.Math;

namespace TornadoScript.ScriptMain.Utility
{
    public class ShapeTestResult
    {
        public bool DidHit { get; private set; }
        public int HitEntity { get; private set; }
        public Vector3 HitPosition { get; private set; }
        public Vector3 HitNormal { get; private set; }
        public materials HitMaterial { get; private set; }

        public ShapeTestResult(bool didHit, int hitEntity, Vector3 hitPosition, Vector3 hitNormal, materials hitMaterial)
        {
            DidHit = didHit;
            HitEntity = hitEntity;
            HitPosition = hitPosition;
            HitNormal = hitNormal;
            HitMaterial = hitMaterial;
        }
    }

    public static class ShapeTestEx
    {
        public unsafe static ShapeTestResult RunShapeTest(Vector3 start, Vector3 end, Entity ignoreEntity, IntersectOptions options)
        {
            var shapeTest = Function.Call<int>(Hash._CAST_RAY_POINT_TO_POINT,
                start.X, start.Y, start.Z, end.X, end.Y, end.Z, (int)options, ignoreEntity, 7);

            bool didHit;

            int result, handle;

            float[] hitPosition = new float[6], hitNormal = new float[6];

            int material;

            fixed (float* position = hitPosition)
            fixed (float* normal = hitNormal)
            {
                result = Function.Call<int>((Hash)0x65287525D951F6BE, shapeTest, &didHit, position, normal, &material, &handle);
            }

            return new ShapeTestResult(didHit, handle, new Vector3(hitPosition[0], hitPosition[2], hitPosition[4]),
                new Vector3(hitNormal[0], hitNormal[2], hitNormal[4]), (materials)material);
        }
    }

    public enum materials
    {
        none = -1,
        unk = -1775485061,
        concrete = 1187676648,
        concrete_pothole = 359120722,
        concrete_dusty = -1084640111,
        tarmac = 282940568,
        tarmac_painted = -1301352528,
        tarmac_pothole = 1886546517,
        rumble_strip = -250168275,
        breeze_block = -954112554,
        rock = -840216541,
        rock_mossy = -124769592,
        stone = 765206029,
        cobblestone = 576169331,
        brick = 1639053622,
        marble = 1945073303,
        paving_slab = 1907048430,
        sandstone_solid = 592446772,
        sandstone_brittle = 1913209870,
        sand_loose = -1595148316,
        sand_compact = 510490462,
        sand_wet = 909950165,
        sand_track = -1907520769,
        sand_underwater = -1136057692,
        sand_dry_deep = 509508168,
        sand_wet_deep = 1288448767,
        ice = -786060715,
        ice_tarmac = -1931024423,
        snow_loose = -1937569590,
        snow_compact = -878560889,
        snow_deep = 1619704960,
        snow_tarmac = 1550304810,
        gravel_small = 951832588,
        gravel_large = 2128369009,
        gravel_deep = -356706482,
        gravel_train_track = 1925605558,
        dirt_track = -1885547121,
        mud_hard = -1942898710,
        mud_pothole = 312396330,
        mud_soft = 1635937914,
        mud_underwater = -273490167,
        mud_deep = 1109728704,
        marsh = 223086562,
        marsh_deep = 1584636462,
        soil = -700658213,
        clay_hard = 1144315879,
        clay_soft = 560985072,
        grass_long = -461750719,
        grass = 1333033863,
        grass_short = -1286696947,
        hay = -1833527165,
        bushes = 581794674,
        twigs = -913351839,
        leaves = -2041329971,
        woodchips = -309121453,
        tree_bark = -1915425863,
        metal_solid_small = -1447280105,
        metal_solid_medium = -365631240,
        metal_solid_large = 752131025,
        metal_hollow_small = 15972667,
        metal_hollow_medium = 1849540536,
        metal_hollow_large = -583213831,
        metal_chainlink_small = 762193613,
        metal_chainlink_large = 125958708,
        metal_corrugated_iron = 834144982,
        metal_grille = -426118011,
        metal_railing = 2100727187,
        metal_duct = 1761524221,
        metal_garage_door = -231260695,
        metal_manhole = -754997699,
        wood_solid_small = -399872228,
        wood_solid_medium = 555004797,
        wood_solid_large = 815762359,
        wood_solid_polished = 126470059,
        wood_floor_dusty = -749452322,
        wood_hollow_small = 1993976879,
        wood_hollow_medium = -365476163,
        wood_hollow_large = -925419289,
        wood_chipboard = 1176309403,
        wood_old_creaky = 722686013,
        wood_high_density = -1742843392,
        wood_lattice = 2011204130,
        ceramic = -1186320715,
        roof_tile = 1755188853,
        roof_felt = -1417164731,
        fibreglass = 1354180827,
        tarpaulin = -642658848,
        plastic = -2073312001,
        plastic_hollow = 627123000,
        plastic_high_density = -1625995479,
        plastic_clear = -1859721013,
        plastic_hollow_clear = 772722531,
        plastic_high_density_clear = -1338473170,
        fibreglass_hollow = -766055098,
        rubber = -145735917,
        rubber_hollow = -783934672,
        linoleum = 289630530,
        laminate = 1845676458,
        carpet_solid = 669292054,
        carpet_solid_dusty = 158576196,
        carpet_floorboard = -1396484943,
        cloth = 122789469,
        plaster_solid = -574122433,
        plaster_brittle = -251888898,
        cardboard_sheet = 236511221,
        cardboard_box = -1409054440,
        paper = 474149820,
        foam = 808719444,
        feather_pillow = 1341866303,
        polystyrene = -1756927331,
        leather = -570470900,
        tvscreen = 1429989756,
        slatted_blinds = 673696729,
        glass_shoot_through = 937503243,
        glass_bulletproof = 244521486,
        glass_opaque = 1500272081,
        perspex = -1619794068,
        car_metal = -93061983,
        car_plastic = 2137197282,
        car_softtop = -979647862,
        car_softtop_clear = 2130571536,
        car_glass_weak = 1247281098,
        car_glass_medium = 602884284,
        car_glass_strong = 1070994698,
        car_glass_bulletproof = -1721915930,
        car_glass_opaque = 513061559,
        water = 435688960,
        blood = 5236042,
        oil = -634481305,
        petrol = -1634184340,
        fresh_meat = 868733839,
        dried_meat = -1445160429,
        emissive_glass = 1501078253,
        emissive_plastic = 1059629996,
        vfx_metal_electrified = -309134265,
        vfx_metal_water_tower = 611561919,
        vfx_metal_steam = -691277294,
        vfx_metal_flame = 332778253,
        phys_no_friction = 1666473731,
        phys_golf_ball = -1693813558,
        phys_tennis_ball = -256704763,
        phys_caster = -235302683,
        phys_caster_rusty = 2016463089,
        phys_car_void = 1345867677,
        phys_ped_capsule = -291631035,
        phys_electric_fence = -1170043733,
        phys_electric_metal = -2013761145,
        phys_barbed_wire = -1543323456,
        phys_pooltable_surface = 605776921,
        phys_pooltable_cushion = 972939963,
        phys_pooltable_ball = -748341562,
        buttocks = 483400232,
        thigh_left = -460535871,
        shin_left = 652772852,
        foot_left = 1926285543,
        thigh_right = -236981255,
        shin_right = -446036155,
        foot_right = -1369136684,
        spine0 = -1922286884,
        spine1 = -1140112869,
        spine2 = 1457572381,
        spine3 = 32752644,
        clavicle_left = -1469616465,
        upper_arm_left = -510342358,
        lower_arm_left = 1045062756,
        hand_left = 113101985,
        clavicle_right = -1557288998,
        upper_arm_right = 1501153539,
        lower_arm_right = 1777921590,
        hand_right = 2000961972,
        neck = 1718294164,
        head = -735392753,
        animal_default = 286224918,
        car_engine = -1916939624,
        puddle = 999829011,
        concrete_pavement = 2015599386,
        brick_pavement = -1147361576,
        phys_dynamic_cover_bound = -2047468855,
        vfx_wood_beer_barrel = 998201806,
        wood_high_friction = -2140087047,
        rock_noinst = 127813971,
        bushes_noinst = 1441114862,
        metal_solid_road_surface = -729112334,
        stunt_ramp_surface = -2088174996,
        temp_01 = 746881105,
        temp_02 = -1977970111,
        temp_03 = 1911121241,
        temp_04 = 1923995104,
        temp_05 = -1393662448,
        temp_06 = 1061250033,
        temp_07 = -1765523682,
        temp_08 = 1343679702,
        temp_09 = 1026054937,
        temp_10 = 63305994,
        temp_11 = 47470226,
        temp_12 = 702596674,
        temp_13 = -1637485913,
        temp_14 = -645955574,
        temp_15 = -1583997931,
        temp_16 = -1512735273,
        temp_17 = 1011960114,
        temp_18 = 1354993138,
        temp_19 = -801804446,
        temp_20 = -2052880405,
        temp_21 = -1037756060,
        temp_22 = -620388353,
        temp_23 = 465002639,
        temp_24 = 1963820161,
        temp_25 = 1952288305,
        temp_26 = -1116253098,
        temp_27 = 889255498,
        temp_28 = -1179674098,
        temp_29 = 1078418101,
        temp_30 = 13626292
    }
}
