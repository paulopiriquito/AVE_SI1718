using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Net.WebSockets;


public interface IGetter {
    string GetValueAsString(object target);
}

public abstract class AbstractGetterObject : IGetter {

    public abstract Object PropertyValue(object target);
    
    public string GetValueAsString(object target)
    {
        Object propValue = PropertyValue(target);
        if (propValue == null)
        {
            return "";
        }
        return propValue.ToString();
    }

}

public class HtmlEmit {


    public static IGetter EmitGetter(PropertyInfo p, Object obj)
    {
        Type klass = obj.GetType();
        AssemblyName aName = new AssemblyName("DynamicGetterFrom");
        AssemblyBuilder ab =
            AppDomain.CurrentDomain.DefineDynamicAssembly(
                aName,
                AssemblyBuilderAccess.RunAndSave);

        // For a single-module assembly, the module name is usually
        // the assembly name plus an extension.
        ModuleBuilder mb =
            ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

        TypeBuilder tb = mb.DefineType(
            "Getter",
             TypeAttributes.Public,
             typeof(AbstractGetterObject));

        MethodBuilder methodBuilder = tb.DefineMethod(
                "PropertyValue",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot,
                typeof(AbstractGetterObject).GetMethod("PropertyValue").ReturnType, // Return Type 
                new Type[] { typeof(object)} // Types of arguments
            );
        
        ILGenerator il = methodBuilder.GetILGenerator();
        il = methodBuilder.GetILGenerator();
        MethodInfo mi = p.GetGetMethod(true);
        il.Emit(OpCodes.Ldarg_1);          // push target
        il.Emit(OpCodes.Castclass, klass); // castclass
        il.Emit(OpCodes.Callvirt, mi);
        if (p.PropertyType.IsValueType)
            il.Emit(OpCodes.Box, p.PropertyType); // box
        il.Emit(OpCodes.Ret);              // ret       // ret


        // Finish the type.
        Type t = tb.CreateType();
        //ab.Save(aName.Name + ".dll");
        return (IGetter) Activator.CreateInstance(t);
    }

}