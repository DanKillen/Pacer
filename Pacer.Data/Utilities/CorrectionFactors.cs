using System.Collections.Generic;


namespace Pacer.Utilities
{
    // The correction factors are derived from Elmer Sterken's research paper, 'Endurance and Age: Evidence from Long-Distance Running Data'.
    // The paper provides a scientifically reliable estimate for performance changes based on age and gender.
    // For more details, refer to https://core.ac.uk/download/pdf/6909056.pdf

    public static class CorrectionFactors
    {
        public static readonly Dictionary<int, (double FiveKFactor, double HalfMarathonFactor, double MarathonFactor)> MaleFactors = new()
    {
        {18, (0.946, 0.854, 0.801)},
        {19, (0.958, 0.866, 0.815)},
        {20, (0.968, 0.877, 0.828)},
        {21, (0.976, 0.887, 0.839)},
        {22, (0.983, 0.895, 0.848)},
        {23, (0.989, 0.901, 0.856)},
        {24, (0.994, 0.907, 0.862)},
        {25, (0.997, 0.911, 0.867)},
        {26, (0.999, 0.914, 0.871)},
        {27, (1.0, 0.916, 0.874)},
        {28, (1.0, 0.916, 0.876)},
        {29, (0.999, 0.916, 0.877)},
        {30, (0.998, 0.916, 0.877)},
        {31, (0.995, 0.914, 0.876)},
        {32, (0.992, 0.911, 0.875)},
        {33, (0.988, 0.908, 0.872)},
        {34, (0.984, 0.905, 0.869)},
        {35, (0.979, 0.9, 0.866)},
        {36, (0.974, 0.896, 0.862)},
        {37, (0.968, 0.89, 0.857)},
        {38, (0.962, 0.885, 0.852)},
        {39, (0.956, 0.879, 0.847)},
        {40, (0.949, 0.873, 0.841)},
        {41, (0.943, 0.866, 0.835)},
        {42, (0.936, 0.86, 0.829)},
        {43, (0.928, 0.853, 0.823)},
        {44, (0.921, 0.846, 0.816)},
        {45, (0.914, 0.839, 0.809)},
        {46, (0.906, 0.831, 0.802)},
        {47, (0.899, 0.824, 0.795)},
        {48, (0.891, 0.817, 0.788)},
        {49, (0.884, 0.809, 0.781)},
        {50, (0.876, 0.802, 0.774)},
        {51, (0.868, 0.794, 0.766)},
        {52, (0.861, 0.787, 0.759)},
        {53, (0.854, 0.78, 0.752)},
        {54, (0.846, 0.772, 0.744)},
        {55, (0.839, 0.765, 0.737)},
        {56, (0.832, 0.758, 0.73)},
        {57, (0.824, 0.75, 0.722)},
        {58, (0.817, 0.743, 0.715)},
        {59, (0.81, 0.736, 0.707)},
        {60, (0.803, 0.728, 0.699)},
        {61, (0.795, 0.721, 0.692)},
        {62, (0.788, 0.713, 0.684)},
        {63, (0.781, 0.706, 0.676)},
        {64, (0.773, 0.698, 0.668)},
        {65, (0.766, 0.69, 0.659)},
        {66, (0.758, 0.682, 0.651)},
        {67, (0.75, 0.673, 0.642)},
        {68, (0.742, 0.665, 0.633)},
        {69, (0.733, 0.656, 0.623)},
        {70, (0.725, 0.647, 0.613)},
        {71, (0.715, 0.637, 0.603)},
        {72, (0.706, 0.627, 0.592)},
        {73, (0.696, 0.616, 0.581)},
        {74, (0.685, 0.605, 0.568)},
        {75, (0.673, 0.593, 0.556)},
        {76, (0.661, 0.58, 0.542)},
        {77, (0.648, 0.566, 0.527)},
        {78, (0.634, 0.552, 0.512)},
        {79, (0.62, 0.536, 0.495)},
        {80, (0.604, 0.519, 0.478)},
        {81, (0.587, 0.502, 0.459)},
        {82, (0.569, 0.483, 0.439)},
        {83, (0.549, 0.462, 0.417)},
        {84, (0.528, 0.440, 0.394)},
        {85, (0.506, 0.417, 0.370)}
    };

        public static readonly Dictionary<int, (double FiveKFactor, double HalfMarathonFactor, double MarathonFactor)> FemaleFactors = new()
    {
        {18, (0.953, 0.847, 0.802)},
        {19, (0.963, 0.858, 0.815)},
        {20, (0.972, 0.868, 0.826)},
        {21, (0.979, 0.876, 0.836)},
        {22, (0.985, 0.884, 0.844)},
        {23, (0.990, 0.890, 0.852)},
        {24, (0.994, 0.895, 0.858)},
        {25, (0.997, 0.899, 0.863)},
        {26, (0.999, 0.901, 0.867)},
        {27, (1.000, 0.903, 0.870)},
        {28, (1.000, 0.904, 0.872)},
        {29, (0.999, 0.904, 0.874)},
        {30, (0.998, 0.904, 0.874)},
        {31, (0.996, 0.902, 0.873)},
        {32, (0.993, 0.900, 0.872)},
        {33, (0.989, 0.897, 0.870)},
        {34, (0.985, 0.893, 0.867)},
        {35, (0.980, 0.889, 0.864)},
        {36, (0.974, 0.884, 0.860)},
        {37, (0.968, 0.879, 0.855)},
        {38, (0.962, 0.873, 0.850)},
        {39, (0.955, 0.867, 0.845)},
        {40, (0.948, 0.860, 0.838)},
        {41, (0.940, 0.853, 0.832)},
        {42, (0.932, 0.845, 0.825)},
        {43, (0.924, 0.837, 0.817)},
        {44, (0.916, 0.829, 0.809)},
        {45, (0.907, 0.821, 0.801)},
        {46, (0.898, 0.812, 0.793)},
        {47, (0.888, 0.803, 0.784)},
        {48, (0.879, 0.794, 0.775)},
        {49, (0.869, 0.784, 0.766)},
        {50, (0.859, 0.775, 0.756)},
        {51, (0.850, 0.765, 0.747)},
        {52, (0.840, 0.755, 0.737)},
        {53, (0.829, 0.745, 0.727)},
        {54, (0.819, 0.735, 0.717)},
        {55, (0.809, 0.724, 0.706)},
        {56, (0.799, 0.714, 0.696)},
        {57, (0.788, 0.703, 0.685)},
        {58, (0.778, 0.693, 0.675)},
        {59, (0.767, 0.682, 0.664)},
        {60, (0.757, 0.671, 0.653)},
        {61, (0.746, 0.661, 0.642)},
        {62, (0.736, 0.650, 0.630)},
        {63, (0.725, 0.639, 0.619)},
        {64, (0.715, 0.628, 0.608)},
        {65, (0.704, 0.617, 0.596)},
        {66, (0.693, 0.606, 0.584)},
        {67, (0.682, 0.594, 0.572)},
        {68, (0.671, 0.583, 0.560)},
        {69, (0.660, 0.571, 0.548)},
        {70, (0.649, 0.559, 0.536)},
        {71, (0.638, 0.548, 0.523)},
        {72, (0.626, 0.535, 0.510)},
        {73, (0.614, 0.523, 0.497)},
        {74, (0.602, 0.511, 0.483)},
        {75, (0.590, 0.498, 0.470)},
        {76, (0.578, 0.484, 0.455)},
        {77, (0.565, 0.471, 0.441)},
        {78, (0.552, 0.457, 0.426)},
        {79, (0.538, 0.443, 0.410)},
        {80, (0.524, 0.428, 0.395)},
        {81, (0.510, 0.413, 0.378)},
        {82, (0.495, 0.397, 0.361)},
        {83, (0.480, 0.380, 0.343)},
        {84, (0.463, 0.363, 0.325)},
        {85, (0.447, 0.345, 0.305)}
    };
    }

}