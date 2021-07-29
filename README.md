# dedsharp

An attempt to rewrite the [ded Editor](https://github.com/tsoding/ded) in C# targetting .Net Core. Why? just because.


While SharpFont is fully compatible with and runs on 64-bit Windows, it relies on a patch for FreeType to do this. This patch is already included in [SharpFont.Dependencies/freetype2]. You do not need to worry about this as a user of the library. If you are compiling FreeType from source, you can find the patch and instructions at the same location.
