// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.Tests.Mocks;

/// <summary>
/// A dummy external DB context used during tests.
/// </summary>
public sealed class ExternalDbContextMock : DbContext
{
}
