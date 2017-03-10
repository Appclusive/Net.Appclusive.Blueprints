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
using System.Diagnostics.Contracts;
using System.Linq;
using Net.Appclusive.Public.Messaging;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Net.Appclusive.Workflows.IoC.Conventions
{
    public class MessagingClientConvention : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            Contract.Assert(null != types);
            Contract.Assert(null != registry);

            types
                .FindTypes(TypeClassification.Concretes | TypeClassification.Closed)
                .Where(TypeFilter)
                .ToList()
                .ForEach(type =>
                {
                    registry.For(type).Use(type);
                });
        }

        public static Func<Type, bool> TypeFilter = 
            t => !string.IsNullOrWhiteSpace(t.Namespace)
                 // ReSharper disable once AssignNullToNotNullAttribute
                && typeof(IMessagingClient).IsAssignableFrom(t);
    }
}
