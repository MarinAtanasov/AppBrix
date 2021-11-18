// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Lifecycle;

/// <summary>
/// Interface used for modules and objects which should make runtime changes to the application:
/// Registering objects, events, etc.
/// The application should call the methods of the modules,
/// modules should call its module objects' methods.
/// </summary>
public interface IApplicationLifecycle
{
    /// <summary>
    /// Makes runtime changes required for the object to function.
    /// </summary>
    /// <param name="context">The initialization context.</param>
    void Initialize(IInitializeContext context);

    /// <summary>
    /// Reverts any changes my by the Initialize method.
    /// Unregisters and uninitializes all of its inner components.
    /// </summary>
    void Uninitialize();
}
