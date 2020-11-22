using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SRServer.Models;

namespace SRServer.Controller.Hubs
{
    public class ConnectionHub : Hub<IConnectionHub>
    {
        private readonly List<User> _Users;
        private readonly List<UserCall> _UserCalls;
        //private readonly List<CallOffer> _CallOffers;

        public ConnectionHub(List<User> users, List<UserCall> userCalls)
        {
            _Users = users;
            _UserCalls = userCalls;
            //_CallOffers = callOffers;
        }

        public async override Task OnConnectedAsync()
        {
            await Clients.Caller.SendConnectionId(Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Hang up any calls the user is in
            // await HangUp(); // Gets the user from "Context" which is available in the whole hub
            //afkar close hagup for now;

            // Remove the user
            _Users.RemoveAll(u => u.ConnectionId == Context.ConnectionId);

            // Send down the new user list to all clients
            await SendUserListUpdate();

            await base.OnDisconnectedAsync(exception);
        }

        public void Disconnected()
        {
            Context.Abort();
        }

        public async Task Join(string username)
        {
            // Add the new user
            _Users.Add(new User
            {
                Username = username,
                ConnectionId = Context.ConnectionId
            });

            // Send down the new list to all clients
            await SendUserListUpdate();
            //afkar
            //await Task.FromResult<List<User>>(_Users);
        }

        public async Task CallUser(User targetConnectionId)
        {
            var callingUser = _Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
            var targetUser = _Users.SingleOrDefault(u => u.ConnectionId == targetConnectionId.ConnectionId);

            // Make sure the person we are trying to call is still here
            if (targetUser == null)
            {
                // If not, let the caller know
                await Clients.Caller.CallDeclined(targetConnectionId, "The user you called has left.");
                return;
            }

            // And that they aren't already in a call
            if (GetUserCall(targetUser.ConnectionId) != null)
            {
                await Clients.Caller
                    .CallDeclined(targetConnectionId, string.Format("{0} is already in a call.", targetUser.Username));
                return;
            }

            // They are here, so tell them someone wants to talk
            await Clients.Client(targetConnectionId.ConnectionId)
                .IncomingCall(callingUser);

        }

        public async Task CallDeclined(User decliningUser, string reason)
        {
            await Clients.Client(decliningUser.ConnectionId).CallDeclined(decliningUser,
                $"did not accept your call {decliningUser.Username}");
        }

        public async Task AnswerCall(bool acceptCall, User targetConnectionId)
        {
            var callingUser = _Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
            var targetUser = _Users.SingleOrDefault(u => u.ConnectionId == targetConnectionId.ConnectionId);

            // This can only happen if the server-side came down and clients were cleared, while the user
            // still held their browser session.
            if (callingUser == null)
            {
                return;
            }

            // Make sure the original caller has not left the page yet
            if (targetUser == null)
            {
                await Clients.Caller.CallEnded(targetConnectionId, "The other user in your call has left.");
                return;
            }

            // Send a decline message if the callee said no
            if (acceptCall == false)
            {
                await Clients.Client(targetConnectionId.ConnectionId).CallDeclined(callingUser, string.Format("{0} did not accept your call.", callingUser.Username));
                return;
            }


            // And finally... make sure the user hasn't accepted another call already
            if (GetUserCall(targetUser.ConnectionId) != null)
            {
                // And that they aren't already in a call
                await Clients.Caller.CallDeclined(targetConnectionId, string.Format("{0} chose to accept someone elses call instead of yours :(", targetUser.Username));
                return;
            }

            // Create a new call to match these folks up
            _UserCalls.Add(new UserCall
            {
                Users = new List<User> { callingUser, targetUser }
            });

            // Tell the original caller that the call was accepted
            await Clients.Client(targetConnectionId.ConnectionId).CallAccepted(callingUser);

            // Update the user list, since thes two are now in a call
            await SendUserListUpdate();
        }
        public async Task HangUp()
        {
            var callingUser = _Users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);

            if (callingUser == null)
            {
                return;
            }

            var currentCall = GetUserCall(callingUser.ConnectionId);

            // Send a hang up message to each user in the call, if there is one
            if (currentCall != null)
            {
                foreach (var user in currentCall.Users.Where(u => u.ConnectionId != callingUser.ConnectionId))
                {
                    await Clients.Client(user.ConnectionId).CallEnded(callingUser, string.Format("{0} has hung up.", callingUser.Username));
                }

                // Remove the call from the list if there is only one (or none) person left.  This should
                // always trigger now, but will be useful when we implement conferencing.
                currentCall.Users.RemoveAll(u => u.ConnectionId == callingUser.ConnectionId);
                if (currentCall.Users.Count < 2)
                {
                    _UserCalls.Remove(currentCall);
                }
            }

            await SendUserListUpdate();
        }

        public async Task Offer(string CallerConnectionId, string TargetOffer)
        {
            await Clients.Client(CallerConnectionId).OfferBack(TargetOffer);
        }

        public async Task AnswerOffer(string TargetConnectionId, string CallerOffer)
        {
            await Clients.Client(TargetConnectionId).AnswerBack(CallerOffer);
        }

        public async Task IceCandidate(string ConnectionId, string Candidate)
        {
            await Clients.Client(ConnectionId).IceCandidate(Candidate);
        }

        #region Private Helpers

        private async Task SendUserListUpdate()
        {
            //_Users.ForEach(u => u.InCall = (GetUserCall(u.ConnectionId) != null));
            //await Clients.All.UpdateUserList(_Users);
            //Afkar
            //var users = _Users.Where(u => u.ConnectionId != Context.ConnectionId).ToList();
            await Clients.All.UpdateUserList(_Users);
        }

        private UserCall GetUserCall(string connectionId)
        {
            var matchingCall =
                _UserCalls.SingleOrDefault(uc =>
                uc.Users.SingleOrDefault(
                    u => u.ConnectionId == connectionId) != null);
            return matchingCall;
        }

        #endregion






    }
    public interface IConnectionHub
    {
        Task UpdateUserList(List<User> userList);
        Task CallAccepted(User acceptingUser);
        Task CallDeclined(User decliningUser, string reason);
        Task IncomingCall(User callingUser);
        Task CallEnded(User signalingUser, string signal);
        Task SendConnectionId(string ConnectionId);
        //afkar
        //Task SendData(Browser.Types.MediaStream stream);
        Task OfferBack(string TargetOffer);
        Task AnswerBack(string TargetOffer);
        Task IceCandidate(string Candidate);
    }
}
