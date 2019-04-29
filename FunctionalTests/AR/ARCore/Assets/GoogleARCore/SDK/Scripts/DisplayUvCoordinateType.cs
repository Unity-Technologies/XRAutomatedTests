﻿//-----------------------------------------------------------------------
// <copyright file="DisplayUvCoordinateType.cs" company="Google">
//
// Copyright 2019 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore
{
    /// <summary>
    /// Types of UV coordinates used for the display.
    /// </summary>
    public enum DisplayUvCoordinateType
    {
        /// <summary>
        /// The background texture used to display the pass-through camera available from
        /// <see cref="GoogleARCore.Frame.CameraImage.Texture"/>.
        /// </summary>
        BackgroundTexture,

        /// <summary>
        /// The background image bytes used for CPU access to the pass-through camera image available from
        /// <see cref="GoogleARCore.Frame.CameraImage.AcquireCameraImageBytes"/>.
        /// </summary>
        BackgroundImage,

        /// <summary>
        /// The Unity Screen.
        /// </summary>
        UnityScreen,
    }
}
