using System.Collections.Generic;

namespace APIServer.MasterData
{
    public class AttendanceRewardData
    {
        public int day_seq { get; set; }
        public int reward_key { get; set; }
        public int reward_qty { get; set; }
    }

    public class CharacterData
    {
        public int char_key { get; set; }
        public string char_name { get; set; }
        public string char_grade { get; set; }
        public int stat_run { get; set; }
        public int stat_power { get; set; }
        public int stat_jump { get; set; }
        public int game_key { get; set; }
    }

    public class SkinData
    {
        public int skin_key { get; set; }
        public string skin_name { get; set; }
        public int char_key { get; set; }
        public int skin_bonus_percent { get; set; }
    }

    public class CostumeData
    {
        public int costume_key { get; set; }
        public string costume_name { get; set; }
        public int costume_type { get; set; }
        public int set_key { get; set; }
    }

    public class CostumeSetData
    {
        public int set_key { get; set; }
        public int char_key { get; set; }
        public string set_name { get; set; }
        public int set_bonus_percent { get; set; }
        public int char_bonus_percent { get; set; }
    }

    public class FoodData
    {
        public int food_key { get; set; }
        public string food_name { get; set; }
        public int game_key { get; set; }
    }

    public class SkillData
    {
        public int skill_key { get; set;}
        public int act_prob_percent { get; set; }
        public int char_key { get; set; } = 0;
    }

    public class GachaRewardData
    {
        public GachaRewardInfo gachaRewardInfo { get; set; }
        public List<GachaReward> gachaRewardList { get; set; }
    }

    public class GachaRewardInfo
    {
        public int gacha_reward_key { get; set; }
        public int char_prob_percent { get; set; }
        public int skin_prob_percent { get; set; }
        public int costume_prob_percent { get; set; }
        public int food_prob_percent { get; set; }
        public int food_gear_prob_percent { get; set; }
        public int gacha_count { get; set; }
        public string gacha_reward_name { get; set; }
    }

    public class GachaReward
    {
        public int reward_key { get; set; }
        public string reward_type { get; set; }
    }
}
