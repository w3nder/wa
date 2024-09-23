// Decompiled with JetBrains decompiler
// Type: System.Lightup
// Assembly: System.Net.Http.Extensions, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4532C01-CAE1-4FEB-922A-3FFFB1361F31
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Extensions.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Extensions.xml

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

#nullable disable
namespace System
{
  internal abstract class Lightup
  {
    private static readonly Type[] EmptyTypes = new Type[0];
    private readonly Type _type;

    protected Lightup(Type type) => this._type = type;

    protected bool TryGet<T>(ref Delegate storage, string propertyName, out T value)
    {
      return this.TryCall<T>(ref storage, "get_" + propertyName, out value);
    }

    protected bool TryGet<TI, TV>(
      ref Delegate storage,
      TI instance,
      string propertyName,
      out TV value)
    {
      return this.TryCall<TI, TV>(ref storage, instance, "get_" + propertyName, out value);
    }

    protected T Get<T>(ref Delegate storage, string propertyName)
    {
      return this.Call<T>(ref storage, "get_" + propertyName);
    }

    protected void Set<T>(ref Delegate storage, string propertyName, T value)
    {
      this.Call<T>(ref storage, "set_" + propertyName, value);
    }

    protected void Set<TI, TV>(ref Delegate storage, TI instance, string propertyName, TV value)
    {
      this.Call<TI, TV>(ref storage, instance, "set_" + propertyName, value);
    }

    protected bool TrySet<TI, TV>(
      ref Delegate storage,
      TI instance,
      string propertyName,
      TV value)
    {
      return this.TryCall<TI, TV>(ref storage, instance, "set_" + propertyName, value);
    }

    protected bool TryCall<T>(ref Delegate storage, string methodName, out T returnValue)
    {
      Func<T> methodAccessor = this.GetMethodAccessor<Func<T>>(ref storage, methodName);
      if (methodAccessor == null)
      {
        returnValue = default (T);
        return false;
      }
      try
      {
        returnValue = methodAccessor();
        return true;
      }
      catch (NotImplementedException ex)
      {
      }
      catch (NotSupportedException ex)
      {
      }
      returnValue = default (T);
      return false;
    }

    protected bool TryCall<TI, TV>(
      ref Delegate storage,
      TI instance,
      string methodName,
      out TV returnValue)
    {
      Func<TI, TV> methodAccessor = this.GetMethodAccessor<Func<TI, TV>>(ref storage, methodName, false);
      if (methodAccessor == null)
      {
        returnValue = default (TV);
        return false;
      }
      try
      {
        returnValue = methodAccessor(instance);
        return true;
      }
      catch (NotImplementedException ex)
      {
      }
      catch (NotSupportedException ex)
      {
      }
      returnValue = default (TV);
      return false;
    }

    protected T Call<T>(ref Delegate storage, string methodName)
    {
      Func<T> methodAccessor = this.GetMethodAccessor<Func<T>>(ref storage, methodName);
      if (methodAccessor == null)
        throw new InvalidOperationException();
      return methodAccessor();
    }

    protected void Call(ref Delegate storage, string methodName)
    {
      Action methodAccessor = this.GetMethodAccessor<Action>(ref storage, methodName);
      if (methodAccessor == null)
        throw new InvalidOperationException();
      methodAccessor();
    }

    protected bool TryCall<TI, TV>(
      ref Delegate storage,
      TI instance,
      string methodName,
      TV parameter)
    {
      Action<TI, TV> methodAccessor = this.GetMethodAccessor<Action<TI, TV>>(ref storage, methodName, false);
      if (methodAccessor == null)
        return false;
      try
      {
        methodAccessor(instance, parameter);
        return true;
      }
      catch (NotImplementedException ex)
      {
      }
      catch (NotSupportedException ex)
      {
      }
      return false;
    }

    protected bool TryCall<TI, TV1, TV2>(
      ref Delegate storage,
      TI instance,
      string methodName,
      TV1 parameter1,
      TV2 parameter2)
    {
      Action<TI, TV1, TV2> methodAccessor = this.GetMethodAccessor<Action<TI, TV1, TV2>>(ref storage, methodName, false);
      if (methodAccessor == null)
        return false;
      try
      {
        methodAccessor(instance, parameter1, parameter2);
        return true;
      }
      catch (NotImplementedException ex)
      {
      }
      catch (NotSupportedException ex)
      {
      }
      return false;
    }

    protected void Call<TI, TV>(
      ref Delegate storage,
      TI instance,
      string methodName,
      TV parameter)
    {
      Action<TI, TV> methodAccessor = this.GetMethodAccessor<Action<TI, TV>>(ref storage, methodName, false);
      if (methodAccessor == null)
        throw new InvalidOperationException();
      methodAccessor(instance, parameter);
    }

    protected void Call<T>(ref Delegate storage, string methodName, T parameter)
    {
      Action<T> methodAccessor = this.GetMethodAccessor<Action<T>>(ref storage, methodName);
      if (methodAccessor == null)
        throw new InvalidOperationException();
      methodAccessor(parameter);
    }

    protected static T Create<T>(params object[] parameters)
    {
      return Lightup.CreateActivator<T>(((IEnumerable<object>) parameters).Select<object, Type>((Func<object, Type>) (p => p.GetType())).ToArray<Type>())(parameters);
    }

    protected object Create(ref Func<object[], object> storage, params object[] parameters)
    {
      Func<object[], object> constructor = this.GetConstructor(ref storage, parameters);
      if (constructor == null)
        throw new InvalidOperationException();
      return constructor(parameters);
    }

    protected virtual object GetInstance() => throw new NotImplementedException();

    private static Func<object[], T> CreateActivator<T>(Type[] argumentTypes)
    {
      return typeof (T).GetConstructor(argumentTypes) == null ? (Func<object[], T>) null : (Func<object[], T>) (arguments => (T) Activator.CreateInstance(typeof (T), arguments));
    }

    private Func<object[], object> CreateActivator(Type[] argumentTypes)
    {
      return this._type.GetConstructor(argumentTypes) == null ? LightupServices.ConstructorNotFound : (Func<object[], object>) (arguments => Activator.CreateInstance(this._type, arguments));
    }

    protected Func<object[], object> GetConstructor(
      ref Func<object[], object> storage,
      object[] parameters)
    {
      if (storage == null)
      {
        Func<object[], object> activator = this.CreateActivator(((IEnumerable<object>) parameters).Select<object, Type>((Func<object, Type>) (p => p.GetType())).ToArray<Type>());
        Interlocked.CompareExchange<Func<object[], object>>(ref storage, activator, (Func<object[], object>) null);
      }
      return !(storage == LightupServices.ConstructorNotFound) ? storage : (Func<object[], object>) null;
    }

    private Delegate CreateMethodAccessor(Type type, string name, bool bindInstance = true)
    {
      if (this._type == null)
        return (Delegate) null;
      Type[] methodArgumentTypes = LightupServices.GetMethodArgumentTypes(type, bindInstance);
      MethodInfo method = this._type.GetMethod(name, methodArgumentTypes);
      return method == null ? (Delegate) null : LightupServices.CreateDelegate(type, bindInstance ? this.GetInstance() : (object) null, method);
    }

    protected T GetMethodAccessor<T>(ref Delegate storage, string name, bool bindInstance = true)
    {
      return (T) this.GetMethodAccessor(ref storage, typeof (T), name, bindInstance);
    }

    protected Delegate GetMethodAccessor(
      ref Delegate storage,
      Type type,
      string name,
      bool bindInstance = true)
    {
      if ((object) storage == null)
      {
        Delegate methodAccessor = this.CreateMethodAccessor(type, name, bindInstance);
        Interlocked.CompareExchange<Delegate>(ref storage, methodAccessor, (Delegate) null);
      }
      return !(storage == LightupServices.NotFound) ? storage : (Delegate) null;
    }
  }
}
