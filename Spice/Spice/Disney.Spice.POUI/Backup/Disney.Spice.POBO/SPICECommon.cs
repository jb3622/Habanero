using System;
using System.Collections.Generic;
using System.Text;
using Disney.Menu;

namespace Disney.Spice.POBO
{
    public class SPICECommon
    {
        //Class for common DB /Params 

        //TODO extract all the common params ie dbparamref /menu etc to a common class 

        private ASNA.VisualRPG.Runtime.Database _dbparamref;

        public ASNA.VisualRPG.Runtime.Database Dbparamref
        {
            get { return _dbparamref; }

        }
        private Disney.Menu.Users _username;

        public Disney.Menu.Users Username
        {
            get { return _username; }

        }
        private Disney.Menu.Environments _penvironment;

        public Disney.Menu.Environments Penvironment
        {
            get { return _penvironment; }

        }
        private string _defaultMarket;

        public string DefaultMarket
        {
            get { return _defaultMarket; }

        }

        public SPICECommon(ASNA.VisualRPG.Runtime.Database dbparamref, Disney.Menu.Users username, Disney.Menu.Environments paramenv, string market)
        {

            _dbparamref = dbparamref;
            _username = username;
            _penvironment = _penvironment;
            _defaultMarket = market;


        }
    }
}