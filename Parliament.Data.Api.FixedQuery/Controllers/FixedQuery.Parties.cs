﻿namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph party_index()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName ;
        :commonsCount ?commonsCount ;
        :lordsCount ?lordsCount .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT ?party ?partyName (COUNT(?mp) AS ?commonsCount) (COUNT(?lord) AS ?lordsCount) WHERE {
        ?party
            a :Party ;
            :partyHasPartyMembership ?partyMembership ;
            :partyName ?partyName .
        OPTIONAL {
            ?partyMembership :partyMembershipHasPartyMember ?person .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?person :memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL {
			    ?incumbency a :SeatIncumbency ;
                			:incumbencyHasMember ?mp .
            }
            OPTIONAL {
			    ?incumbency a :HouseIncumbency ;
                  			:incumbencyHasMember ?lord .
            }
        }
      }
      GROUP BY ?party ?partyName
   }
   UNION {
        SELECT DISTINCT ?firstLetter WHERE {
	        ?s a :Party ;
                :partyHasPartyMembership ?partyMembership ;
            	:partyName ?partyName .

          	BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
         }
   }
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_by_id(string party_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?name ;
        :commonsCount ?commonsCount ;
        :lordsCount ?lordsCount .
}
WHERE {
    SELECT ?party ?name (COUNT(?mp) AS ?commonsCount) (COUNT(?lord) AS ?lordsCount)
    WHERE {
        BIND(@id AS ?party)
       	?party a :Party ;
	           :partyName ?name .
        OPTIONAL {
            ?party :partyHasPartyMembership ?partyMembership .
    	  	FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
    	  	?partyMembership :partyMembershipHasPartyMember ?member .
    	  	?member :memberHasIncumbency ?incumbency .
    	  	FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL {
		        ?incumbency a :SeatIncumbency ;
                			:incumbencyHasMember ?mp .
            }
            OPTIONAL {
                ?incumbency a :HouseIncumbency ;
                            :incumbencyHasMember ?lord .
            }
        }
    }
    GROUP BY ?party ?name
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteSingle(query);

        }

        [HttpGet]
        public Graph party_by_initial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName ;
        :commonsCount ?commonsCount ;
        :lordsCount ?lordsCount .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT ?party ?partyName (COUNT(?mp) AS ?commonsCount) (COUNT(?lord) AS ?lordsCount) WHERE {
        ?party
            a :Party ;
            :partyHasPartyMembership ?partyMembership ;
            :partyName ?partyName .
        OPTIONAL {
            ?partyMembership :partyMembershipHasPartyMember ?person .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?person :memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL {
			    ?incumbency a :SeatIncumbency ;
                  			:incumbencyHasMember ?mp .
            }
            OPTIONAL {
			    ?incumbency a :HouseIncumbency ;
                  		    :incumbencyHasMember ?lord .
            }
        }
        FILTER STRSTARTS(LCASE(?partyName), LCASE(@initial)) .
     }
     GROUP BY ?party ?partyName
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
            ?s a :Party ;
                :partyHasPartyMembership ?partyMembership;
                :partyName ?partyName.

            BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_current()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName ;
        :commonsCount ?commonsCount ;
        :lordsCount ?lordsCount .
}
WHERE {
    SELECT ?party ?partyName (COUNT(?mp) AS ?commonsCount) (COUNT(?lord) AS ?lordsCount) WHERE {
        ?incumbency a :Incumbency .
        FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
        ?incumbency :incumbencyHasMember ?person .
        ?person :partyMemberHasPartyMembership ?partyMembership .
        FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
        ?partyMembership :partyMembershipHasParty ?party .
        ?party :partyName ?partyName .
        OPTIONAL {
	        ?incumbency a :SeatIncumbency ;
                	    :incumbencyHasMember ?mp .
        }
        OPTIONAL {
		    ?incumbency a :HouseIncumbency ;
                		:incumbencyHasMember ?lord .
        }
    }
    GROUP BY ?party ?partyName
}
";
            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_a_to_z()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    [ :value ?firstLetter ]
}
WHERE {
    SELECT DISTINCT ?firstLetter
    WHERE {
        ?s
            a :Party ;
           :partyHasPartyMembership ?partyMembership ;
           :partyName ?partyName .
        BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_current_a_to_z()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    [ :value ?firstLetter ]
}
WHERE {
    SELECT DISTINCT ?firstLetter
    WHERE {
        ?incumbency a :Incumbency .
        FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
        ?incumbency :incumbencyHasMember ?person .
        ?person :partyMemberHasPartyMembership ?partyMembership .
        FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
        ?partyMembership :partyMembershipHasParty ?party .
        ?party :partyName ?partyName .
        BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_lookup(string property, string value) => base.LookupInternal("Party", property, value);

        [HttpGet]
        public Graph party_by_substring(string substring)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName ;
        :commonsCount ?commonsCount ;
        :lordsCount ?lordsCount .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT ?party ?partyName (COUNT(?mp) AS ?commonsCount) (COUNT(?lord) AS ?lordsCount) WHERE {
        ?party
            a :Party ;
            :partyHasPartyMembership ?partyMembership ;
            :partyName ?partyName .
        OPTIONAL {
            ?partyMembership :partyMembershipHasPartyMember ?person .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?person :memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL {
			    ?incumbency a :SeatIncumbency ;
                			:incumbencyHasMember ?mp .
            }
            OPTIONAL {
			    ?incumbency a :HouseIncumbency ;
                  			:incumbencyHasMember ?lord .
            }
        }
        FILTER CONTAINS(LCASE(?partyName), LCASE(@substring))

      }
        GROUP BY ?party ?partyName
   }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {

            ?s a :Party ;
                :partyHasPartyMembership ?partyMembership;
            	:partyName ?partyName.

              BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
         }
   }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("substring", substring);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_members(string party_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipEndDate ?partyMembershipEndDate ;
        :partyMembershipHasParty ?party .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@partyid AS ?party)
        ?party
            a :Party ;
            :partyName ?partyName .
        OPTIONAL {
            ?party :partyHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasPartyMember ?person .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
                	:memberHasIncumbency ?incumbency .
            OPTIONAL
            {
                ?incumbency a :HouseIncumbency .
                BIND(?incumbency AS ?houseIncumbency)
                OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        	}
        	OPTIONAL {
                ?incumbency a :SeatIncumbency .
                BIND(?incumbency AS ?seatIncumbency)
                ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
          }
	    }
      }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
            BIND(@partyid AS ?party)

            ?party a :Party ;
                   :partyHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasPartyMember ?person .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

            BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_current_members(string party_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipEndDate ?partyMembershipEndDate ;
        :partyMembershipHasParty ?party .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@partyid AS ?party)
        ?party
            a :Party ;
            :partyName ?partyName .
        OPTIONAL {
            ?party :partyHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasPartyMember ?person .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
                	:memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL
            {
                ?incumbency a :HouseIncumbency .
                BIND(?incumbency AS ?houseIncumbency)
                OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        	}
        	OPTIONAL {
                ?incumbency a :SeatIncumbency .
                BIND(?incumbency AS ?seatIncumbency)
                ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
          }
	    }
      }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
            BIND(@partyid AS ?party)

            ?party a :Party ;
                   :partyHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasPartyMember ?person .
            ?person :memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

            BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_members_by_initial(string party_id, string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipEndDate ?endDate ;
        :partyMembershipHasParty ?party .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@partyid AS ?party)
        ?party
            a :Party ;
            :partyName ?partyName .
        OPTIONAL {
            ?party :partyHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasPartyMember ?person .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?endDate . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
                	:memberHasIncumbency ?incumbency .
            OPTIONAL
            {
                ?incumbency a :HouseIncumbency .
                BIND(?incumbency AS ?houseIncumbency)
                OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        	}
        	OPTIONAL {
                ?incumbency a :SeatIncumbency .
                BIND(?incumbency AS ?seatIncumbency)
                ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
          }
          FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
        }
    }
}
UNION {
        SELECT DISTINCT ?firstLetter WHERE {
            BIND(@partyid AS ?party)

            ?party a :Party ;
                   :partyHasPartyMembership ?partyMembership.
            ?partyMembership :partyMembershipHasPartyMember ?person .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

            BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_members_a_to_z(string party_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    _:x :value ?firstLetter .
    ?party
    	a :Party ;
    	:partyName ?partyName .
}
WHERE {
	{
	    SELECT DISTINCT ?firstLetter
	    WHERE {
	        BIND(@partyid AS ?party)
	        ?party
	            a :Party ;
	            :partyHasPartyMembership ?partyMembership ;
	            :partyName ?partyName .
	        ?partyMembership :partyMembershipHasPartyMember ?person .
	        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
	        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
	    }
  }
  UNION
  {
  	BIND(@partyid AS ?party)
    ?party
    	a :Party ;
    	:partyName ?partyName .
	}
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_current_members_by_initial(string party_id, string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipEndDate ?partyMembershipEndDate ;
        :partyMembershipHasParty ?party .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@partyid AS ?party)
        ?party
            a :Party ;
            :partyName ?partyName .
        OPTIONAL {
            ?party :partyHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasPartyMember ?person .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
                	:memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL
            {
                ?incumbency a :HouseIncumbency .
                BIND(?incumbency AS ?houseIncumbency)
                OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        	}
        	OPTIONAL {
                ?incumbency a :SeatIncumbency .
                BIND(?incumbency AS ?seatIncumbency)
                ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
          }
          FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
	    }
    }
}
UNION {
        SELECT DISTINCT ?firstLetter WHERE {
            BIND(@partyid AS ?party)

            ?party a :Party ;
                   :partyHasPartyMembership ?partyMembership.
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership. }
            ?partyMembership :partyMembershipHasPartyMember ?person.
            ?person :memberHasIncumbency ?incumbency .

             FILTER NOT EXISTS { ?incumbency a :PastIncumbency. }
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

            BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_current_members_a_to_z(string party_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    _:x :value ?firstLetter .
    ?party
    	a :Party ;
    	:partyName ?partyName .
}
WHERE {
	{
    SELECT DISTINCT ?firstLetter
    WHERE {
        BIND(@partyid AS ?party)
        ?party
            a :Party ;
            :partyName ?partyName ;
            :partyHasPartyMembership ?partyMembership .
        FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
        ?partyMembership :partyMembershipHasPartyMember ?person .
        ?person :memberHasIncumbency ?incumbency .
        FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
    }
  }
  UNION
  {
    BIND(@partyid AS ?party)
	?party
	    a :Party ;
	    :partyName ?partyName ;
  }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }
    }
}
