﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Suddath.Helix.JobMgmt.Models
{
    public static class NameTranslator
    {
        public static string StorageUser
        {
            get
            {
                return "Storage Manager";
            }
        }

        public static Dictionary<string, string> repo = new Dictionary<string, string>
        {
            {"STEVEC","Steve Crooks"},
            {"JASON","Jason Bell"},
            {"TERESAB",$"{StorageUser}"},
            {"DOTTYS","Dotty Schmitt"},
            {"LINDAW",$"{StorageUser}"},
            {"PATSYC","Patsy Conn"},
            {"BINGO","Bingo Bell"},
            {"JAMESB","James Bell"},
            {"PETERB","Peter Bowsher"},
            {"EHEALY","Elizabeth Healy"},
            {"JMORISON","Jack Morison"},
            {"MARYS","Mary Sortal"},
            {"DAWNE","Dawn Elkin"},
            {"RAMONS","Ramon Sierra"},
            {"HEATHERR","Heather Randolph"},
            {"JOSEM","Jose Marrero"},
            {"ANGELAK",$"{StorageUser}"},
            {"CHRISJ","Chris Jenkins"},
            {"LAURENK","Lauren Kolesar"},
            {"DEBBIEK","Debbie Kummoung"},
            {"APLAZA","Andres Plaza"},
            {"NLUONG","Nelly Luong"},
            {"ROBERTH","Robert Hammer"},
            {"LISAK","Lisa Karam"},
            {"KLOPEZ","Kirk Lopez"},
            {"TWILLIAMS","Teresa Williams"},
            {"PMARTIN",$"{StorageUser}"},
            {"TWHITE","Timothy White"},
            {"DALLEN","Devorah Allen"},
            {"RKAMINSKI","Rosalia Kaminski"},
            {"RMELANSON","Ryan Melanson"},
            {"RSTRICKLAND","Robin Stickland"},
            {"EMISKELLY","Erica Miskelly"},
            {"MCHAVEZ","Monica Chavez"},
            {"JHALL","Juanita Hall"},
            {"MVIELGUTH","Mery Vielguth"},
            {"MWILSON","Mike Wilson"},
            {"LNASSAU","Lita Nassau"},
            {"LSHEFFIELD","Lindsey Sheffield"},
            {"ROSIEL","Rosie Lara"},
            {"CSITZ","Carissa Sitz"},
            {"MSPENCER","Melissa Spencer"},
            {"RLAMADRID","Rod Lamadrid"},
            {"ESONG","Emily Song"},
            {"EBACA","Elba Baca"},
            {"EPONCE","Emma Ponce"},
            {"JDITORE","James Ditore"},
            {"GOSTEEN","Gordon O'Steen"},
            {"JMACIK","Janna Macik"},
            {"BDRUMM","Briana Lee Drumm"},
            {"KMULLINS","Kenneth Mullins"},
            {"DCOLLINS",$"{StorageUser}"},
            {"SLOWE","Sarah Lowe"},
            {"AMBARB","Ambar Brust"},
            {"MRENZELLA","Michael Renzella"},
            {"MWARREN","Martha Warren"},
            {"PHILW","Phil Watts"},
            {"CZEIGLER","Corrie Zeigler"},
            {"PVELIZ","Priscilla Veliz"},
            {"SPARK","Sunny Park"},
            {"NPANTARAS","Natalia Pantaras"},
            {"DDRAYTON","Daisy Drayton"},
            {"LCRU","-GONZALEZ	Luzette Cruz"},
            {"EDAVIS","Eldridge Davis"},
            {"TNELSON","Trenna Nelson"},
            {"SBARTELS","Susan Bartels"},
            {"SGONGORA","Sasha Gongora"},
            {"MIKEJOHNSON","Mike Johnson"},
            {"CSIEGEL","Colleen Siegel"},
            {"LALAIMO","Louis Alaimo"},
            {"CANTHONY","Christine Anthony"},
            {"HMACKAY","Hatice Mackay"},
            {"SPATEL","Sonal Patel"},
            {"LREGISTER","Lori Register"},
            {"DABDELALIM","Dina Abd El Alim"},
            {"RBADELLA","Rich Badella"},
            {"CCAIN","Char Cain"},
            {"CCOLLINS","Christina Strickland"},
            {"TDOBBS","Timothy Dobbs"},
            {"SMAY","Suzanne May"},
            {"TBURACCHIO","Trevor Buracchio"},
            {"GHOWARTH","Grant Howarth"},
            {"SGRAHAM","Stephen Graham"},
            {"CHANCOCK","Casey Hancock"},
            {"EHERMANN","Erin Hermann"},
            {"KRUSSELL","Kathryn Russell"},
            {"YWASHINGTON","Yvonda Washington"},
            {"KHUME","Kathleen Hume"},
            {"WBUCKMASTER","Wendy Buckmaster"},
            {"ZWALKER","Zach Walker"},
            {"CDROSDICK","Courtney Drosdick"},
            {"JCROWLEY","Jennifer Crowley"},
            {"ATAYLOR",$"{StorageUser}"},
            {"MBRODRIGUEZ","Marybell Rodriguez"},
            {"JSTANINGER","Jean Staninger"},
            {"DEA2011","Special for DEA"},
            {"DRAFTIS","Douglas Raftis"},
            {"KCOUCH","Kathy Couch"},
            {"CPEREZ","Carolyn Perez"},
            {"ROBERTG","Robert Garcia"},
            {"KOCONNOR","Keila O'Connor"},
            {"RMONTOUR","Ryan Montour"},
            {"HSCULLY","Heather Scully"},
            {"AGUEIMUNDE","Antonio Gueimunde"},
            {"DBUSH","Debra Bush"},
            {"JLUONG","John Luong"},
            {"JTRAN","John Tran"},
            {"MQUIGG","Mark Quigg"},
            {"ABREWER","Amy Brewer"},
            {"SRENEBERG","Sonya Reneberg"},
            {"DROTONDARO","Domenico Rotondaro"},
            {"TGLEASON","Teela Gleason"},
            {"NGILBERT","Neidra Gilbert"},
            {"AKLINE","Amy Kline"},
            {"NICOLETTEALLEN",""},
            {"ATIMMINS","Alessandra Timmins"},
            {"NALLEN","Nicolette Allen"},
            {"VOSBORNE","Vanessa Osborne"},
            {"BRUTLEDGE","Beth Rutledge"},
            {"DNELSON","Deanna Nelson"},
            {"ELEARY","Erin Leary"},
            {"TABITHAWHITE","Tabitha White"},
            {"MWHITE","Michael White"},
            {"RNOVAK","Rebecca Novak"},
            {"MERYV","Mery Vielguth"},
            {"JANNAM","Janna Macik"},
            {"MERMLICH","Michelle Ermlich"},
            {"ELIGHTFOOT","Emma Worley"},
            {"CBANKS","Carley Banks"},
            {"KDINKINS","Kim Dinkins"},
            {"BINGOLOW","Low priority user"},
            {"BCESSAC","Blythe Cessac"},
            {"DLIMBAUGH","Daffney Limbaugh"},
            {"MLUGO","Mercedes Lugo"},
            {"DMARSON","Dave Marson"},
            {"MMOONEY",$"{StorageUser}"},
            {"JRIOS","Jessie Rios"},
            {"DANIELKING","Daniel King"},
            {"LISAW",$"{StorageUser}"},
            {"LSWANSON","Larry Swanson"},
            {"ASILOYAN","Anzhelika Siloyan"},
            {"BGINDY","Brandi Gindy"},
            {"PJONES","Paul Jones"},
            {"JSTADLER","Jim Stadler"},
            {"MJAMES","Michael James"},
            {"RCHOLLET","Renee Chollet"},
            {"KPAPUGA","KJ Papuga"},
            {"JSUMPTER","Jason Sumpter"},
            {"GVIRTUE","Grant Virtue"},
            {"LCLANTON","Lisa Clanton"},
            {"SWITHEY","Sarah Withey"},
            {"APOMARANSKI","Andrea Pomaranski"},
            {"KTORRES","Katherine Torres"},
            {"MBLALOCK","Michelle Blalock"},
            {"TRACEYSMITH","Tracey Smith"},
            {"DMILAKIS","Debbie Milakis"},
            {"ZGAMIL","Zachary El Gamil"},
            {"LMILLS","Lisa Mills"},
            {"MDIAZ","Maricelis Diaz"},
            {"CSTRICKLAND","Christina Strickland"},
            {"MMELANSON","Mery Melanson"},
            {"SSHAMI","Sawsan Shami"},
            {"EOLIVER","Elizabeth Oliver"},
            {"ASWANSEY","Angela Swansey"},
            {"MDAINGERFIELD","Maryann Dangerfield"},
            {"SJENKINS","Sonya Jenkins"},
            {"LERDELYI","Laura Erdelyi"},
            {"DHUDGENS","David Hudgens"},
            {"TCOLLINS","Tara Collins"},
            {"AZUREUSER","Tim"},
            {"SCLARK","Staniqua Clark"},
            {"SARAHW","Sarah Withey"},
            {"CHRISTIANWAYNE","Christian Wayne"},
            {"ESINGLETON","Elizabeth Singleton"},
            {"DANACACCHIONE","Dana Cacchione"},
            {"DONNA_REINKE","Donna Reinke"},
            {"JSCHULTZ","Joseph Schultz"},
            {"DLAWRENCE","Daniel Lawrence"},
            {"MICHELEWALL","Michele Wall"},
            {"WEB_USER_PORTAL","For PHP Web Portal"},
            {"WEB_USER_API","For PHP Web API"},
            {"MKURTZ","Matthew Kurtz"},
            {"DMARRIOTT","Destiny Marriott"},
            {"KLAWRENCE","Katie Morton"},
            {"MICHELLELEWIS","Michelle Lewis"},
            {"ZSTEPHENSON","Zachary Stephenson"},
            {"PSANCHEZ","Punnarai Sanchez"},
            {"SAMANTHACLARKE","Samantha Clarke"},
            {"NTICHY","Nick Tichy"},
            {"CSTARMER","Chloe Starmer"},
            {"MWALL","Michele Wall"},
            {"LMCCARTY","Lona McCarty"},
            {"JMCCAULEY","Jennifer McCauley"},
            {"SPOMAGER","Scott Pomager"},
            {"SSMITH","Sue Smith"},
            {"CSTAHR","Crystal Stahr"},
            {"ALLISONCOOK","Allison Cook"},
            {"CJENKINS","Christopher Jenkins"},
            {"JHERRIN","Jamie Herrin"},
            {"EDINES","Emily Dines"},
            {"JSTEWARD","Jill Steward"},
            {"AJOHNSON","Andre.Johnson"},
            {"SBROWN","Scott Brown"},
            {"MGRAY","Michael Gray"},
            {"SKAUSHAL","Sanjay Kaushal"},
            {"JKO","Joana Ko"},
            {"PLIMPANTSIS","Pauline Limpantsis"},
            {"NPINSON","Nancy Pinson"},
            {"LOTALVARO","Lucas Otalvaro"},
            {"JAMIN","Jay Amin"},
            {"JLORD",$"{StorageUser}"},
            {"EPROCTOR","Elizabeth Miller"},
            {"TCOHEE","Tracie Cohee"},
            {"CRODRIGUES","Francisco Rodrigues"},
            {"HAILEYRUTLEDGE","Hailey Rutledge"},
            {"TKADUK","Taras Kaduk"},
            {"SCARDONE","Stephanie Cardone"},
            {"ELEE","Elizabeth Lee"},
            {"JRANDOLPH","Julianna Randolph"},
            {"MCHASE","Michael Chase"},
            {"ANDREWHANRAHAN","Andrew Hanrahan"},
            {"DLAMBERS","Dan Lambers"},
            {"CMOROSE","Carissa Morose"},
            {"CSANCHEZ","Carlos Sanchez"},
            {"AMOORE","Angel Moore"},
            {"YTHIBODEAUX",$"{StorageUser}"},
            {"SONEILL","Scott O'Neill"},
            {"ABROOKS","Ashley Brooks"},
            {"GCAMACHO","George Camacho"},
            {"BBARRANTES","Bobbie Barrantes"},
            {"JNIXON","Juanita Nixon"},
            {"ESANCHEZ","Enrique Sanchez"},
            {"MGUERRA","Melissa Guerra"},
            {"CEDWARDS","Carolyn Edwards"},
            {"ACORTEZ","Anna Cortez"},
            {"ABROWN","Antrelle Brown"},
            {"JENNIFERSCALANTE","Jennifer Scalante"},
            {"SHARONBENTFORD","Sharon Bentford"},
            {"MDARWISH","Marisol Wild"},
            {"STEPHANYBALLASSO","Maha Darwish"},
            {"LOLIVOS",$"{StorageUser}"},
            {"CKNILL","Caryn Knill"},
            {"MFISHER2","Matthew Fisher"},
            {"SSAWANT","Christine Anthony"},
            {"DDATTA","Carolyn Perez"},
            {"DCLEMENTS","Leda Olivos"},
            {"ANNETTECRAWFORD","Annette Crawford"},
            {"Angela.Lafronza", $"{StorageUser}" },
            {"STORAGE", $"{StorageUser}" },
            {"RIGARTA", $"{StorageUser}" },
            {"LBOWMAN", $"{StorageUser}" },
            {"KATIES", $"{StorageUser}" },
            {"JMARRERO", "Jose G Marrero" },
            {"IJIMENEZ", $"{StorageUser}" },
            {"ALAFRONZA", $"{StorageUser}" }
        };
    }
}