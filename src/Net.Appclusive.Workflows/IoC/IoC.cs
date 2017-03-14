/**
 * Copyright 2017 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Text;
using biz.dfch.CS.Commons.Diagnostics;
using Net.Appclusive.Workflows.IoC.Registries;
using StructureMap;

namespace Net.Appclusive.Workflows.IoC
{
    public class IoC
    {
        private static readonly object _syncRoot = new object();

        private static readonly HashSet<Type> _registries = new HashSet<Type>();

        public static void AddRegistry<T>()
            where T : Registry
        {
            lock (_syncRoot)
            {
                Contract.Assert(!_defaultContainer.IsValueCreated);
                Contract.Assert(!_registries.Contains(typeof(T)));

                _registries.Add(typeof(T));
            }

        }

        private static readonly Lazy<Container> _defaultContainer = new Lazy<Container>(() =>
        {
            Contract.Ensures(null != Contract.Result<Container>());

            var registry = new Registry();
            registry.IncludeRegistry<DefaultRegistry>();

            var methodInfo = registry.GetType().GetMethod(nameof(Registry.IncludeRegistry), Type.EmptyTypes);
            Contract.Assert(null != methodInfo);
            Contract.Assert(methodInfo.IsGenericMethod);
            foreach (var type in _registries)
            {
                var genericMethodInfo = methodInfo.MakeGenericMethod(type);
                Contract.Assert(null != genericMethodInfo);

                genericMethodInfo.Invoke(registry, null);
            }

            var container = new Container(registry);

            var message = new StringBuilder(container.WhatDoIHave()).Append(container.WhatDidIScan()).ToString();
            var traceSource = Logger.Get(Net.Appclusive.Public.Constants.Logging.TraceSourceName.WEB_API);
            traceSource.TraceEvent(TraceEventType.Verbose, (int) Net.Appclusive.Public.Constants.Logging.EventId.Default, message);

            try
            {
                container.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                traceSource.TraceException(ex);
                throw;
            }

            return container;
        });

        public static Container DefaultContainer => _defaultContainer.Value;


        public static string AssemblyDirectory
        {
            get
            {
                var uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
                return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            }
        }
    }
}
