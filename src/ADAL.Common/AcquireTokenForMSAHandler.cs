﻿//----------------------------------------------------------------------
// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
// Apache License 2.0
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//----------------------------------------------------------------------

using System;
using System.Text;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
    class AcquireTokenForMSAHandler : AcquireTokenHandlerBase
    {
        private readonly UserAssertion userAssertion;

        public AcquireTokenForMSAHandler(Authenticator authenticator, TokenCache tokenCache, string resource, string clientId, UserAssertion userAssertion, bool callSync)
            : base(authenticator, tokenCache, resource, new ClientKey(clientId), TokenSubjectType.UserPlusClient, callSync)
        {
            if (userAssertion == null)
            {
                throw new ArgumentNullException("userAssertion");
            }

            if (string.IsNullOrWhiteSpace(userAssertion.AssertionType))
            {
                var ex = new ArgumentException(AdalErrorMessage.UserCredentialAssertionTypeEmpty, "userAssertion");
                Logger.LogException(this.CallState, ex);
                throw ex;
            }

            this.userAssertion = userAssertion;
            this.DisplayableId = userAssertion.UserName;
        }

        protected override void AddAditionalRequestParameters(RequestParameters requestParameters)
        {
            requestParameters[OAuthParameter.GrantType] = this.userAssertion.AssertionType;
            requestParameters[OAuthParameter.Assertion] = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.userAssertion.Assertion));

            // To request id_token in response
            requestParameters[OAuthParameter.Scope] = OAuthValue.ScopeOpenId;
        }
    }
}
