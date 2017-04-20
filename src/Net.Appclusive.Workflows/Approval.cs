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
using System.Activities;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using biz.dfch.CS.Commons;
using biz.dfch.CS.Commons.Converters;
using Net.Appclusive.Public.Domain.Control;
using Net.Appclusive.Workflows.Public;
using ApprovalAttributes = Net.Appclusive.Public.Constants.Approval;

namespace Net.Appclusive.Workflows
{
    public sealed class Approval : NativeActivity<DictionaryParameters>
    {
        private static readonly ConvertibleBaseDtoConverter _converter = new ConvertibleBaseDtoConverter();
        private static readonly MethodInfo _genericMethodInfo = _converter.GetType().GetMethod
        (
            nameof(ConvertibleBaseDtoConverter.Convert),
            new[] { typeof(DictionaryParameters) }
        );

        private static readonly Dictionary<string, Type> _approvalTypeMap = new Dictionary<string, Type>
        {
            { ApprovalAttributes.Approval0.BASE, typeof(Approvals.Approval0) },
            { ApprovalAttributes.Approval1.BASE, typeof(Approvals.Approval1) },
            { ApprovalAttributes.Approval2.BASE, typeof(Approvals.Approval2) },
            { ApprovalAttributes.Approval3.BASE, typeof(Approvals.Approval3) },
            { ApprovalAttributes.Approval4.BASE, typeof(Approvals.Approval4) },
            { ApprovalAttributes.Approval5.BASE, typeof(Approvals.Approval5) },
            { ApprovalAttributes.Approval6.BASE, typeof(Approvals.Approval6) },
            { ApprovalAttributes.Approval7.BASE, typeof(Approvals.Approval7) },
            { ApprovalAttributes.Approval8.BASE, typeof(Approvals.Approval8) },
            { ApprovalAttributes.Approval9.BASE, typeof(Approvals.Approval9) },
            { ApprovalAttributes.ApprovalA.BASE, typeof(Approvals.ApprovalA) },
            { ApprovalAttributes.ApprovalB.BASE, typeof(Approvals.ApprovalB) },
            { ApprovalAttributes.ApprovalC.BASE, typeof(Approvals.ApprovalC) },
            { ApprovalAttributes.ApprovalD.BASE, typeof(Approvals.ApprovalD) },
            { ApprovalAttributes.ApprovalE.BASE, typeof(Approvals.ApprovalE) },
            { ApprovalAttributes.ApprovalF.BASE, typeof(Approvals.ApprovalF) },
        };

        [RequiredArgument]
        public InArgument<string> ModelName { get; set; }

        [RequiredArgument]
        public InArgument<DictionaryParameters> Configuration { get; set; }

        protected override bool CanInduceIdle => true;

        protected override void Execute(NativeActivityContext context)
        {
            Contract.Assert(null != context);

            var modelName = context.GetValue(ModelName);
            Contract.Assert(!string.IsNullOrWhiteSpace(modelName));

            var configuration = context.GetValue(Configuration);
            Contract.Assert(null != configuration);

            Process(context, null, null);
        }

        public void OnResumeBookmark(NativeActivityContext context, Bookmark bookmark, object obj)
        {
            Process(context, bookmark, obj);
        }

        private void Process(NativeActivityContext context, Bookmark bookmark, object obj)
        {
            Contract.Requires(null != context);

            var modelName = context.GetValue(ModelName);
            Contract.Assert(!string.IsNullOrWhiteSpace(modelName));

            var configuration = context.GetValue(Configuration);
            Contract.Assert(null != configuration);


            // here we do the following:
            // we process all possible approvals by iterating over them (from 0..F)
            // on first invocation (i.e. no bookmark resumption) we process the first Approval we find (eg Approval.0)
            // on subsequent invocation we iterate until we find the approval (derived from the bookmark name)
            // once we found it we skip it and process the next (possible) Approval
            // once all approvals have been processed, we set the result on the Activity so the workflow can be continued
            var hasPendingApproval = false;
            var findingNextApproval = null != bookmark;
            foreach (var approvalMapItem in _approvalTypeMap)
            {
                // if we are on a "Resume" operation and are still looking for the reumed approval we compare the bookmark name with the approval name
                if (null != bookmark && findingNextApproval && bookmark.Name.EndsWith(approvalMapItem.Key))
                {
                    // signal that we found the currently resumed approval
                    findingNextApproval = false;
                    continue;
                }

                // skip current approval if we are still looking for the resumed approval
                if(null != bookmark && findingNextApproval) continue;

                var methodInfo = _genericMethodInfo.MakeGenericMethod(approvalMapItem.Value, typeof(ApprovalAttribute));
                var approvalBase = methodInfo.Invoke(_converter, new object[] { configuration }) as Approvals.ApprovalBase;
                Contract.Assert(null != approvalBase, approvalMapItem.Value.FullName);

                // only process valid approval attributes
                if (!approvalBase.IsValid()) continue;

                // the rest is as usual, create a bookmark and send a tracking record
                var name = WorkflowUtilities.GetApprovalBookmarkName(context.WorkflowInstanceId, context.ActivityInstanceId, approvalMapItem.Key);
                context.CreateBookmark
                (
                    name,
                    OnResumeBookmark
                );

                context.Track(new ApprovalTrackingRecord(context.WorkflowInstanceId)
                {
                    BookmarkName = name,
                });

                // signal that we are still not finished, but created another approval
                hasPendingApproval = true;
                break;
            }

            // save result if all approvals have been processed
            if (null == bookmark || hasPendingApproval) return;

            Result.Set
            (
                context,
                obj as DictionaryParameters ?? new DictionaryParameters()
            );
        }
    }
}
