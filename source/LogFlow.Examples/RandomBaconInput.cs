using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LogFlow.Examples
{
	public class RandomBaconInput : LogInput
	{
		private List<string> _randomBacon = new List<string>()
		{
			@"Bacon ipsum dolor sit amet brisket cow leberkas, kielbasa strip steak landjaeger shank boudin pig short loin hamburger jowl ham shoulder. Andouille pastrami jerky spare ribs. Ham hock sirloin hamburger prosciutto pancetta pork chop flank jowl bacon ball tip turkey ribeye tenderloin drumstick. Pastrami tongue ham hock shankle frankfurter short loin shoulder swine fatback sausage boudin landjaeger salami pork. Beef ribs beef sausage corned beef pork loin pastrami meatloaf hamburger ribeye leberkas. Leberkas ball tip t-bone strip steak pork belly, pork chop chicken.",
			@"Fatback short ribs capicola tenderloin turkey jerky turducken jowl cow. Turkey capicola pancetta tenderloin sausage, biltong swine pork bresaola ribeye strip steak doner leberkas. Corned beef kielbasa andouille sirloin t-bone kevin. Drumstick short ribs flank bacon leberkas kielbasa ground round tail pig spare ribs pork chop. Hamburger kevin beef ribs meatloaf sausage corned beef boudin turducken venison meatball tri-tip bresaola. Corned beef ground round cow brisket, bresaola kielbasa andouille short ribs swine chuck.",
			@"Ribeye porchetta tongue cow ham kevin leberkas jerky. Shank porchetta flank, shankle leberkas ground round ham hock pork belly bacon sausage beef ribs doner corned beef turducken pork. Flank ribeye swine, salami tongue chicken hamburger prosciutto shankle doner shoulder leberkas turducken sausage tri-tip. Doner pastrami prosciutto turducken turkey, pork pork chop kevin ground round rump filet mignon tail brisket drumstick shankle. Prosciutto doner meatball rump tri-tip. Ribeye pork belly short loin shankle pork chop beef ribs. Filet mignon meatball tenderloin salami turkey flank.", 
			@"Spare ribs ham hock short ribs sirloin turducken. Filet mignon shank meatball strip steak biltong beef ribs meatloaf ball tip landjaeger capicola rump pastrami turducken doner jerky. Doner landjaeger fatback flank boudin bresaola strip steak swine tongue brisket cow turkey ball tip jowl. Sausage biltong boudin t-bone pork filet mignon. Fatback pork chop pork belly prosciutto jowl ball tip, meatball biltong kielbasa turkey venison pork loin shank salami shoulder. Chicken tri-tip ribeye corned beef fatback.",
			@"Andouille t-bone ribeye, pastrami tail short loin filet mignon corned beef. Tri-tip pastrami drumstick bresaola pork chop chicken pork loin ham hock boudin kevin pig capicola rump prosciutto. Capicola bacon chicken, ground round biltong frankfurter landjaeger leberkas strip steak meatloaf chuck turkey tongue. Jerky beef tail, pork belly strip steak jowl pork loin. Brisket kevin turducken leberkas flank short loin drumstick pancetta boudin short ribs. Rump jowl jerky biltong tenderloin, turducken doner. Doner tenderloin sirloin sausage andouille porchetta filet mignon kielbasa rump ribeye ham landjaeger beef kevin.",
			@"Flank ham spare ribs pancetta fatback, beef chicken. Pork corned beef jowl pork belly hamburger strip steak spare ribs drumstick jerky frankfurter ham hock cow. Corned beef prosciutto rump sirloin boudin. Pastrami biltong pork belly, pig tail boudin chuck filet mignon ball tip.",
			@"T-bone boudin kielbasa drumstick, short loin brisket chuck turkey spare ribs pork pig short ribs shank. Leberkas shankle chuck boudin prosciutto tail. Pork loin corned beef shank drumstick hamburger pork belly meatloaf boudin swine fatback spare ribs chicken capicola doner. Pig capicola salami pork loin doner beef ribs tail sirloin prosciutto jowl corned beef andouille drumstick rump. Bacon biltong ham meatball pork tenderloin drumstick leberkas. Pastrami ham hock brisket jerky. Jowl shank ground round turkey chuck rump pastrami hamburger pig prosciutto venison.",
			@"Frankfurter beef ribeye hamburger flank, ham hock tongue bacon pork chop capicola ball tip ham pancetta. Sirloin t-bone biltong, meatloaf ground round drumstick capicola pancetta pork. T-bone short ribs swine strip steak pork sausage tail capicola pork belly corned beef tongue pork chop kevin. Short loin turkey pork chop, kielbasa rump hamburger meatloaf meatball cow turducken tri-tip tail strip steak jowl. Tongue pork loin beef, shank prosciutto short loin ham hock leberkas chuck pancetta pastrami andouille.",
			@"Andouille boudin prosciutto, rump ham hock landjaeger pastrami pig shankle corned beef fatback. Brisket chicken shoulder, ground round turkey strip steak meatloaf. Jerky chicken salami sausage, pig pork pastrami shoulder pork chop sirloin brisket. Turducken strip steak salami short ribs filet mignon.",
			@"Kielbasa leberkas tri-tip, cow pancetta biltong bresaola jowl. Salami swine boudin shankle pork belly bacon chicken sirloin hamburger prosciutto corned beef cow jerky kevin beef ribs. Tenderloin andouille pig shank bresaola beef ribs. Beef ribs ham pork loin chuck kielbasa strip steak hamburger shoulder cow andouille. Pastrami doner bacon porchetta meatloaf tail."
		}; 

		public override Result GetLine()
		{
			var result = new Result(LogContext);
			result.Line = _randomBacon[new Random().Next(_randomBacon.Count)];
			result.EventTimeStamp = DateTime.UtcNow;
			result.Json = JObject.Parse("{ Message: \"" + result.Line + "\" } ");
			return result;
		}

		public override void LineIsProcessed(Result result)
		{
		}
	}
}
