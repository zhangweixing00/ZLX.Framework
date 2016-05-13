using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Web;

namespace ZLX.Framework.ToolKit.DP_Dynamic.Core
{
    public class CustomType
    {
        public Object GetInstance(Type type, Dictionary<string, string> dict)
        {
            object instance = Activator.CreateInstance(type);

            foreach (KeyValuePair<string, string> item in dict)
            {
                PropertyInfo info = type.GetProperty(item.Key);
                if (info != null)
                {
                    info.SetValue(instance, item.Value, null);
                }
            }

            return instance;
        }

        /// <summary>
        /// 方法弃用：由于域加载动态库后不能释放，动态库为非托管内存，导致内存泄漏
        /// 替代方法：CreateTypeAdvance
        /// </summary>
        /// <param name="properyNames"></param>
        /// <returns></returns>
        [Obsolete("该方法已停用，请用CreateTypeAdvance代替此方法")]
        public Type CreateType(List<string> properyNames)
        {
            AssemblyName aName = new AssemblyName("DynamicAssembly");
            AssemblyBuilder ab =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    aName,
                    AssemblyBuilderAccess.RunAndSave);

            // For a single-module assembly, the module name is usually
            // the assembly name plus an extension.
            ModuleBuilder mb =
                ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

            TypeBuilder tb = mb.DefineType(
                "MyDynamicType",
                 TypeAttributes.Public);

            foreach (string item in properyNames)
            {
                AddPropery(tb, item);
            }
            return tb.CreateType();
        }

        /// <summary>
        /// 暂用方法
        /// </summary>
        /// <param name="properyNames"></param>
        /// <returns></returns>
        public Type CreateTypeAdvance(List<string> properyNames)
        {
            string assemblyName = GetAssemblyName(properyNames);
            Assembly assembly = null;
            foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (item.IsDynamic && item.FullName.Split(',')[0] == assemblyName)
                {
                    assembly = item;
                    break;
                }
            }

            if (assembly == null)
            {
                AssemblyName aName = new AssemblyName(assemblyName);
                AssemblyBuilder ab =
                    AppDomain.CurrentDomain.DefineDynamicAssembly(
                        aName,
                        AssemblyBuilderAccess.RunAndSave);

                ModuleBuilder mb =
                    ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

                TypeBuilder tb = mb.DefineType(
                    "Star_DynamicType",
                     TypeAttributes.Public);

                foreach (string item in properyNames)
                {
                    AddPropery(tb, item);
                }
                return tb.CreateType();
            }

            Type exsitType = assembly.GetType("Star_DynamicType");
            return exsitType;
        }

        private string GetAssemblyName(List<string> properyNames)
        {
            StringBuilder buider = new StringBuilder();
            foreach (var item in properyNames)
            {
                buider.AppendFormat("{0}_", item);
            }
            string longKey = buider.ToString().Trim('_');
            string shortKey = "";
            if (HttpRuntime.Cache[longKey] == null)
            {
                shortKey = Guid.NewGuid().ToString().Replace('-', '_');
                HttpRuntime.Cache[longKey] = shortKey;
            }

            shortKey = HttpRuntime.Cache[longKey].ToString();
            Console.WriteLine("{0},{1}", shortKey, longKey);
            return shortKey;
        }
        
        public void AddPropery(TypeBuilder builder, string name)
        {
            FieldBuilder field = builder.DefineField("_" + name, typeof(string), FieldAttributes.Private);

            PropertyBuilder propery = builder.DefineProperty(name, PropertyAttributes.HasDefault, typeof(string), null);

            // The property "set" and property "get" methods require a special
            // set of attributes.
            MethodAttributes getSetAttr = MethodAttributes.Public |
                MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // Define the "get" accessor method for Number. The method returns
            // an integer and has no arguments. (Note that null could be 
            // used instead of Types.EmptyTypes)
            MethodBuilder getAccessor = builder.DefineMethod(
                "get_" + name,
                getSetAttr,
                typeof(string),
                Type.EmptyTypes);

            ILGenerator getIL = getAccessor.GetILGenerator();
            // For an instance property, argument zero is the instance. Load the 
            // instance, then load the private field and return, leaving the
            // field value on the stack.
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, field);
            getIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for Number, which has no return
            // type and takes one argument of type int (Int32).
            MethodBuilder setAccessor = builder.DefineMethod(
                "set_" + name, getSetAttr, null,
                new Type[] { typeof(string) });

            ILGenerator numberSetIL = setAccessor.GetILGenerator();
            // Load the instance and then the numeric argument, then store the
            // argument in the field.
            numberSetIL.Emit(OpCodes.Ldarg_0);
            numberSetIL.Emit(OpCodes.Ldarg_1);
            numberSetIL.Emit(OpCodes.Stfld, field);
            numberSetIL.Emit(OpCodes.Ret);

            // Last, map the "get" and "set" accessor methods to the 
            // PropertyBuilder. The property is now complete. 
            propery.SetGetMethod(getAccessor);
            propery.SetSetMethod(setAccessor);
        }
    }
}
