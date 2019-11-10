﻿using NWN.FinalFantasy.Job.Enumeration;

namespace NWN.FinalFantasy.Job.StatRating
{
    internal class RatingE: RatingChart
    {
        public RatingE()
        {
            Set(RatingStat.HP,
                9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
                19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
                29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
                39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
                49, 50, 51, 52, 53, 54, 55, 56, 57, 58,
                59, 60, 61, 62, 63, 64, 65, 66, 67, 68);

            Set(RatingStat.MP,
                9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
                19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
                29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
                39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
                49, 50, 51, 52, 53, 54, 55, 56, 57, 58,
                59, 60, 61, 62, 63, 64, 65, 66, 67, 68);

            Set(RatingStat.AC,
                10, 10, 10, 10, 10, 11, 11, 11, 11, 11,
                12, 12, 12, 12, 12, 13, 13, 13, 13, 13,
                14, 14, 14, 14, 14, 15, 15, 15, 15, 15,
                16, 16, 16, 16, 16, 17, 17, 17, 17, 17,
                18, 18, 18, 18, 18, 19, 19, 19, 19, 19,
                20, 20, 20, 20, 20, 21, 21, 21, 21, 21);

            Set(RatingStat.BAB,
                1, 1, 1, 1, 1, 2, 2, 2, 2, 2,
                3, 3, 3, 3, 3, 4, 4, 4, 4, 4,
                5, 5, 5, 5, 5, 6, 6, 6, 6, 6,
                7, 7, 7, 7, 7, 8, 8, 8, 8, 8,
                9, 9, 9, 9, 9, 10, 10, 10, 10, 10,
                11, 11, 11, 11, 11, 12, 12, 12, 12, 12);

            Set(RatingStat.STR,
                8, 8, 9, 9, 10, 10, 11, 11, 12, 12,
                13, 13, 14, 14, 15, 15, 16, 16, 17, 17,
                18, 18, 19, 19, 20, 20, 21, 21, 22, 22,
                23, 23, 24, 24, 25, 25, 26, 26, 27, 27,
                28, 28, 29, 29, 30, 30, 31, 31, 32, 32,
                33, 33, 34, 34, 35, 35, 36, 36, 37, 37);

            Set(RatingStat.DEX,
                8, 8, 9, 9, 10, 10, 11, 11, 12, 12,
                13, 13, 14, 14, 15, 15, 16, 16, 17, 17,
                18, 18, 19, 19, 20, 20, 21, 21, 22, 22,
                23, 23, 24, 24, 25, 25, 26, 26, 27, 27,
                28, 28, 29, 29, 30, 30, 31, 31, 32, 32,
                33, 33, 34, 34, 35, 35, 36, 36, 37, 37);

            Set(RatingStat.CON,
                8, 8, 9, 9, 10, 10, 11, 11, 12, 12,
                13, 13, 14, 14, 15, 15, 16, 16, 17, 17,
                18, 18, 19, 19, 20, 20, 21, 21, 22, 22,
                23, 23, 24, 24, 25, 25, 26, 26, 27, 27,
                28, 28, 29, 29, 30, 30, 31, 31, 32, 32,
                33, 33, 34, 34, 35, 35, 36, 36, 37, 37);

            Set(RatingStat.WIS,
                8, 8, 9, 9, 10, 10, 11, 11, 12, 12,
                13, 13, 14, 14, 15, 15, 16, 16, 17, 17,
                18, 18, 19, 19, 20, 20, 21, 21, 22, 22,
                23, 23, 24, 24, 25, 25, 26, 26, 27, 27,
                28, 28, 29, 29, 30, 30, 31, 31, 32, 32,
                33, 33, 34, 34, 35, 35, 36, 36, 37, 37);

            Set(RatingStat.INT,
                8, 8, 9, 9, 10, 10, 11, 11, 12, 12,
                13, 13, 14, 14, 15, 15, 16, 16, 17, 17,
                18, 18, 19, 19, 20, 20, 21, 21, 22, 22,
                23, 23, 24, 24, 25, 25, 26, 26, 27, 27,
                28, 28, 29, 29, 30, 30, 31, 31, 32, 32,
                33, 33, 34, 34, 35, 35, 36, 36, 37, 37);

            Set(RatingStat.CHA,
                8, 8, 9, 9, 10, 10, 11, 11, 12, 12,
                13, 13, 14, 14, 15, 15, 16, 16, 17, 17,
                18, 18, 19, 19, 20, 20, 21, 21, 22, 22,
                23, 23, 24, 24, 25, 25, 26, 26, 27, 27,
                28, 28, 29, 29, 30, 30, 31, 31, 32, 32,
                33, 33, 34, 34, 35, 35, 36, 36, 37, 37);
        }
    }
}