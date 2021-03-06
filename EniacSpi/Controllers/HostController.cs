﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EniacSpi.Models;
using EniacSpi.Interfaces;
using EniacSpi.Objects;
using System.Net;
using System.Threading.Tasks;

namespace EniacSpi.Controllers
{
    [Authorize(Roles = "User")]
    public class HostController : Controller
    {

        private static IHostInformation nullSelectedTargetHost = new HostInformation { MAC = "N/A" };
        private static INetworkInformation nullSelectedNetwork= new NetworkInformation
        {
            SSID = "N/A",
            MAC = "N/A",
            Security = "N/A",
            Signal = 0,
            CrackProgressEnd = 1,
            CrackProgressStatus = 0,
            IsCracking = false,
            Password = "N/A"
        };

        public ActionResult Index(string Name)
        {
            var host = HostManager.Current.GetHost(Name);

            if (host == null)
                return RedirectToRoute(new { Action="Index", Controller="Home" });

            IEnumerable<SelectListItem> availableNetworkDropDownList = host.AvailableNetworks.Select(x => new SelectListItem { Text = x.SSID, Value = x.MAC, Selected = x.MAC == host.SelectedNetwork.MAC });
            IEnumerable<SelectListItem> availableTargetHostDropDownList = host.AvailableTargetHosts.Select(x => new SelectListItem { Text = x.MAC, Value = x.MAC, Selected = x == host.SelectedTargetHost});

            var model = new HostViewModel
            {             
                Name = host.Name,
                AvailableNetworkDropDownList = availableNetworkDropDownList ?? Enumerable.Empty<SelectListItem>(),
                AvailableTargetHostsDropDownList = availableTargetHostDropDownList ?? Enumerable.Empty<SelectListItem>(),
                SelectedNetwork = host.SelectedNetwork ?? nullSelectedNetwork,
                SelectedTargetHost = host.SelectedTargetHost ?? nullSelectedTargetHost
            };

            return View(model);
        }


        public ActionResult List()
        {
            IEnumerable<IHost> hosts = HostManager.Current.GetHosts();
            var model = hosts.Select(x => new HostListViewModel { Address = x.Address, Name = x.Name });

            return View(model);
        }

        public ActionResult NetworkInformation(string Name, string selectedMAC)
        {
            var host = HostManager.Current.GetHost(Name);

            if (host == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var selectedNetwork = host.AvailableNetworks.FirstOrDefault(network => network.MAC == selectedMAC);

            HostNetworkInformationModel model = new HostNetworkInformationModel
            {
                SSID = nullSelectedNetwork.SSID,
                MAC = nullSelectedNetwork.MAC,
                Security = nullSelectedNetwork.Security,
                Signal = nullSelectedNetwork.Signal
            };

            if (selectedNetwork != null)
            {
                model = new HostNetworkInformationModel
                {
                    SSID = selectedNetwork.SSID,
                    MAC = selectedNetwork.MAC,
                    Security = selectedNetwork.Security,
                    Signal = selectedNetwork.Signal
                };
            }
            ViewBag.HostName = Name;

            return PartialView(model);
        }
        public ActionResult NetworkInfiltration(string Name, string selectedMAC)
        {
            var host = HostManager.Current.GetHost(Name);

            if (host == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var selectedNetwork = host.AvailableNetworks.FirstOrDefault(network => network.MAC == selectedMAC);

            var model = new HostNetworkInfiltrationModel
            {
                CrackProgressEnd = nullSelectedNetwork.CrackProgressEnd,
                CrackProgressStatus = nullSelectedNetwork.CrackProgressStatus,
                IsCracking = nullSelectedNetwork.IsCracking,
                Password = nullSelectedNetwork.Password
            };

            if (selectedNetwork != null)
            {
                model = new HostNetworkInfiltrationModel
                {
                    CrackProgressStatus = selectedNetwork.CrackProgressStatus,
                    CrackProgressEnd = selectedNetwork.CrackProgressEnd,
                    IsCracking = selectedNetwork.IsCracking,
                    Password = selectedNetwork.Password
                };
            }
            ViewBag.HostName = Name;

            return PartialView(model);
        }

        public ActionResult TargetHostInformation(string Name, string selectedMAC)
        {
            var host = HostManager.Current.GetHost(Name);

            if (host == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var selectedTargetHost = host.AvailableTargetHosts.FirstOrDefault(network => network.MAC == selectedMAC);

            HostTargetHostInformationModel model = new HostTargetHostInformationModel
            {
                MAC = nullSelectedTargetHost.MAC
            };

            if (selectedTargetHost != null)
            {
                model = new HostTargetHostInformationModel
                {
                    MAC = selectedTargetHost.MAC
                };
            }

            return PartialView(model);
        }

        public ActionResult SelectNetwork(string Name, string selectedMAC)
        {
            var host = HostManager.Current.GetHost(Name);

            if (host == null)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

            host.StopCracking();
            host.SelectedNetwork = host.AvailableNetworks.FirstOrDefault(x => x.MAC == selectedMAC);

            return RedirectToAction("Index", new { Name = Name });
        }

        public ActionResult SelectTargetHost(string Name, string selectedMAC)
        {
            var host = HostManager.Current.GetHost(Name);

            if (host == null)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            
            host.SelectedTargetHost = host.AvailableTargetHosts.FirstOrDefault(x => x.MAC == selectedMAC);

            return RedirectToAction("Index", new { Name = Name });
        }

        public async Task<string> StartCracking(string Name)
        {
            var host = HostManager.Current.GetHost(Name);

            if (host == null || host.SelectedNetwork == null)
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError).ToString();

            //start cracking!!
            return await host.StartCracking();
        }

        public ActionResult StopCracking(string Name)
        {
            var host = HostManager.Current.GetHost(Name);

            if (host == null || host.SelectedNetwork == null)
                return RedirectToAction("Index", new { Name = Name });

            //stop cracking!!
            host.StopCracking();

            return RedirectToAction("Index", new { Name = Name });
        }
    }
}