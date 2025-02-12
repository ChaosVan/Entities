using System;

namespace Entities
{
    /// <summary>
    /// Prevents a system from being automatically created and run.
    /// </summary>
    /// <remarks>
    /// By default, all systems (classes derived from <see cref="ComponentSystemBase"/> or <see cref="ISystem"/>) are automatically discovered,
    /// instantiated, and added to the default <see cref="World"/> when that World is created.
    ///
    /// Add this attribute to a system class that you do not want created automatically. Note that the attribute is not
    /// inherited by any subclasses.
    ///
    /// <code>
    /// using Unity.Entities;
    ///
    /// [DisableAutoCreation]
    /// public partial class CustomSystem : SystemBase
    /// { // Implementation... }
    /// </code>
    ///
    /// You can also apply this attribute to an entire assembly to prevent any system class in that assembly from being
    /// created automatically. This is useful for test assemblies containing many systems that expect to be tested
    /// in isolation.
    ///
    /// To declare an assembly attribute, place it in any C# file compiled into the assembly, outside the namespace
    /// declaration:
    /// <code>
    /// using Unity.Entities;
    ///
    /// [assembly: DisableAutoCreation]
    /// namespace Tests{}
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, Inherited = false)]
    public sealed class DisableAutoCreationAttribute : Attribute
    {
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    /// <remarks>Defines where internal Unity systems should be created. The existence of these flags and
    /// the specialized Worlds they represent are subject to change.</remarks>
    [Flags]
    public enum WorldSystemFilterFlags : uint
    {
        /// <summary>
        /// The default <see cref="World"/>.
        /// Systems without a [WorldSystemFilter] attribute are by default placed in this world.
        /// </summary>
        Default = 1 << 0,
        ///// <summary>
        ///// A specialized World created for converting GameObjects to entities.
        ///// </summary>
        //GameObjectConversion = HybridGameObjectConversion | DotsRuntimeGameObjectConversion,
        ///// <summary>
        ///// A specialized World created for optimizing scene rendering.
        ///// </summary>
        //EntitySceneOptimizations = 1 << 2,
        ///// <summary>
        ///// A specialized World created for processing a scene after load.
        ///// </summary>
        //ProcessAfterLoad = 1 << 3,
        ///// <summary>
        ///// Conversion systems that should run for Hybrid.
        ///// Example: Hybrid renderer conversion systems
        ///// </summary>
        //HybridGameObjectConversion = 1 << 4,
        ///// <summary>
        ///// Conversion systems that should run for Dots runtime.
        ///// Example: Tiny renderer conversion systems
        ///// </summary>
        //DotsRuntimeGameObjectConversion = 1 << 5,
        ///// <summary>
        ///// The main World created when running in the Editor.
        ///// Example: Editor LiveConversion system
        ///// </summary>
        //Editor = 1 << 6,
        /// <summary>
        /// Flag to include all system groups defined above as well as systems decorated with [DisableAutoCreation].
        /// </summary>
        All = ~0u
    }


    /// <summary>
    /// For internal use only.
    /// </summary>
    /// <remarks>Defines where internal Unity systems should be created. The existence of these Worlds
    /// is subject to change.</remarks>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public sealed class WorldSystemFilterAttribute : Attribute
    {
        /// <summary>
        /// The World the system belongs in.
        /// </summary>
        public WorldSystemFilterFlags FilterFlags;

        /// <summary>For internal use only.</summary>
        /// <param name="flags">Defines where internal Unity systems should be created.</param>
        public WorldSystemFilterAttribute(WorldSystemFilterFlags flags)
        {
            FilterFlags = flags;
        }
    }
}
