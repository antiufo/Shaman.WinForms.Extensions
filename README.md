# Shaman.WinForms.Extensions

Utilities for WinForms applications.

```csharp
using Shaman.WinForms;

dialogBox.ShowAroundVisualOwner(form);

form.CopyWindowPosition(form2); // Copies the window location/size, and shifts it by a few pixels so that it's visibly different

form.EnsureVisible(); // Moves window to a visible location

// Saving and loading window location setting: takes into account maximization, minimization, etc.
string setting = WindowsFormExtensions.SerializePosition(form);
WindowsFormExtensions.InitWindowLocation(setting, form);

```
