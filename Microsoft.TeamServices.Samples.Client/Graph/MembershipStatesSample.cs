﻿using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamServices.Samples.Client.Graph
{
    [ClientSample(GraphResourceIds.AreaName, GraphResourceIds.Memberships.MembershipStatesResourceName)]
    public class MembershipStatesSample : ClientSample
    {
        /// <summary>
        /// Check whether a descriptor is active or inactive
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public void GetMembershipStateBySubjectDescriptor()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: add the AAD user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADUserByOIDWithVSID");
            GraphUserCreationContext addAADUserContext = new GraphUserOriginIdCreationContext
            {
                OriginId = "e97b0e7f-0a61-41ad-860c-748ec5fcb20b",
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: check Membership state
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipStateBySubjectDescriptor");
            GraphMembershipState membershipState = graphClient.GetMembershipStateAsync(userDescriptor).Result;

            //
            // Part 3: remove the user
            // 
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipStateBySubjectDescriptor-After");
            try
            {
                membershipState = graphClient.GetMembershipStateAsync(userDescriptor).Result;
                if (membershipState.Active) throw new Exception();
            }
            catch (Exception e)
            {
                Context.Log("The deleted user is not disabled!");
            }
        }
    }
}
