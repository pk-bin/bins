using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SciChart.Charting.Visuals;

namespace WpfApplication1
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Ensure SetLicenseKey is called once, before any SciChartSurface instance is created 
            // Check this code into your version-control and it will enable SciChart 
            // for end-users of your application who are not activated


            /*      SciChartSurface.SetRuntimeLicenseKey(@"<LicenseContract>
               <Customer>My Company</Customer>
               <OrderId>123-456</OrderId>
               <LicenseCount>1</LicenseCount>
               <IsTrialLicense>false</IsTrialLicense>
               <SupportExpires>07/06/2013 00:00:00</SupportExpires>
               <KeyCode>ABCDEFG</KeyCode>
             </LicenseContract>"); */

            SciChartSurface.SetRuntimeLicenseKey(@"<LicenseContract>
		<Customer>HURA</Customer>
		<OrderId>ABT160219-1625-79118</OrderId>
		<LicenseCount>1</LicenseCount>
		<IsTrialLicense>false</IsTrialLicense>
		<SupportExpires>02/18/2017 00:00:00</SupportExpires>
		<ProductCode>SC-WPF-2D-PRO</ProductCode>
		<KeyCode>lwAAAAEAAAAenMb0noDRAWQAQ3VzdG9tZXI9SFVSQTtPcmRlcklkPUFCVDE2MDIxOS0xNjI1LTc5MTE4O1N1YnNjcmlwdGlvblZhbGlkVG89MTgtRmViLTIwMTc7UHJvZHVjdENvZGU9U0MtV1BGLTJELVBST2ZMv/aB3hfdLITQCh1wJGQUDlmxlZS+o5u8gEVTz8Rx3WvMz8e3zrsXV12zGHxNig==</KeyCode>
		</LicenseContract>");
        }
    }
}
