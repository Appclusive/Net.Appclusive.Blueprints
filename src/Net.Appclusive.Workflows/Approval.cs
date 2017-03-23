﻿/**
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

using System.Activities;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using biz.dfch.CS.Commons;
using Net.Appclusive.Workflows.Public;

namespace Net.Appclusive.Workflows
{
    public sealed class Approval : NativeActivity<DictionaryParameters>
    {
        [RequiredArgument]
        public InArgument<string> ModelName { get; set; }

        [RequiredArgument]
        public InArgument<DictionaryParameters> Configuration { get; set; }

        protected override bool CanInduceIdle => true;

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            Contract.Assert(null != context);

            var modelName = context.GetValue(ModelName);
            Contract.Assert(!string.IsNullOrWhiteSpace(modelName));

            var configuration = context.GetValue(Configuration);
            Contract.Assert(null != configuration);

            // DFTODO - remove once we get this from order / outside
            configuration.Add("Net.Appclusive.Public.GlobalAttributes.ApproverRoleName.1", "CreatorsOwners");

            foreach (KeyValuePair<string, object> pair in configuration.Where(e => e.Key.StartsWith("Net.Appclusive.Public.GlobalAttributes.ApproverRoleName.")))
            {
                var name = WorkflowUtilities.GetBookmarkName(context.WorkflowInstanceId, context.ActivityInstanceId);
                context.Track(new ApprovalTrackingRecord(context.WorkflowInstanceId)
                {
                    BookmarkName = name,
                });

                context.CreateBookmark
                (
                    name,
                    OnResumeBookmark
                );
            }
        }

        public void OnResumeBookmark(NativeActivityContext context, Bookmark bookmark, object obj)
        {
            // When the Bookmark is resumed, assign its value to  
            // the Result argument.

            var result = obj as DictionaryParameters ?? new DictionaryParameters
            {
                { "key1", "value1" },
                { "key2", 42L },
            };
            Result.Set(context, result);
        }
    }
}
