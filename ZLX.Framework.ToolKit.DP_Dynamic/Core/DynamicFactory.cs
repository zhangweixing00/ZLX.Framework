using System.Collections.Concurrent;
using System;
using System.Reflection;
using System.Reflection.Emit;
namespace ZLX.Framework.ToolKit.DP_Dynamic.Core
{
    public static class DynamicFactory
    {
        private static ConcurrentDictionary<Type, Type> s_dynamicTypes = new ConcurrentDictionary<Type, Type>();

        private static Func<Type, Type> s_dynamicTypeCreator = new Func<Type, Type>(CreateDynamicType);

        public static object ToDynamic(this object entity)
        {
            var entityType = entity.GetType();
            var dynamicType = s_dynamicTypes.GetOrAdd(entityType, s_dynamicTypeCreator);

            var dynamicObject = Activator.CreateInstance(dynamicType);
            foreach (var entityProperty in entityType.GetProperties())
            {
                var value = entityProperty.GetValue(entity, null);
                dynamicType.GetField(entityProperty.Name).SetValue(dynamicObject, value);
            }

            return dynamicObject;
        }

        private static Type CreateDynamicType(Type entityType)
        {
            var asmName = new AssemblyName("DynamicAssembly_" + Guid.NewGuid());
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            var moduleBuilder = asmBuilder.DefineDynamicModule("DynamicModule_" + Guid.NewGuid());

            var typeBuilder = moduleBuilder.DefineType(
                entityType.GetType() + "$DynamicType",
                TypeAttributes.Public);

            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            foreach (var entityProperty in entityType.GetProperties())
            {
                typeBuilder.DefineField(entityProperty.Name, entityProperty.PropertyType, FieldAttributes.Public);
            }

            return typeBuilder.CreateType();
        }
    }
}