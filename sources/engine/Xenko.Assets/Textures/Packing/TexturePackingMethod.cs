// Copyright (c) Xenko contributors (https://xenko.com) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Xenko.Assets.Textures.Packing
{
    /// <summary>
    /// The Heuristic methods used to place sprites in atlas.
    /// </summary>
    public enum TexturePackingMethod
    {
        Best,
        BestShortSideFit,
        BestLongSideFit,
        BestAreaFit,
        BottomLeftRule,
        ContactPointRule
    }
}
