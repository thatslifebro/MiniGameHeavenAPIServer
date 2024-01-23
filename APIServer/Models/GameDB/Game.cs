﻿using System;

namespace APIServer.Models.GameDB
{
    public class GdbGameInfo
    {
        public int game_key { get; set; }
        public int char_key { get; set; }
        public int bestscore { get; set; }
        public DateTime create_dt { get; set; }
        public DateTime new_record_dt { get; set; }
        public DateTime recent_play_dt { get; set; }
        public int bestscore_cur_season { get; set; }
        public int bestscore_prev_season { get; set; }
    }

}
