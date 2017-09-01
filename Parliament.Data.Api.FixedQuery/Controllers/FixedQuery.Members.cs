﻿namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph member_current()
        {
            var queryString = @"
PREFIX :<http://id.ukpds.org/schema/>
CONSTRUCT {
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
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
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?seatIncumbencyStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house ;
        :incumbencyStartDate ?houseIncumbencyStartDate ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?house
        a :House ;
        :houseName ?houseName .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?partyMembershipStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        ?person 
            a :Person ;
            :memberHasIncumbency ?incumbency .
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person :memberHasMemberImage ?image . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        { 
            ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency)
            ?houseIncumbency :houseIncumbencyHasHouse ?house .
            ?house :houseName ?houseName .
            ?houseIncumbency :incumbencyStartDate ?houseIncumbencyStartDate .
            OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        }
        UNION {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency 
                :seatIncumbencyHasHouseSeat ?houseSeat ;
                :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasHouse ?house .
            ?house :houseName ?houseName .
            OPTIONAL {
                ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                ?constituencyGroup :constituencyGroupName ?constituencyName .
            }
        }
        ?person :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasParty ?party .
        ?party :partyName ?partyName .
        ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
        FILTER NOT EXISTS {?incumbency a :PastIncumbency}
      }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
	        ?incumbency a :Incumbency ;
          	FILTER NOT EXISTS { ?incumbency a :PastIncumbency .	}
          	?incumbency :incumbencyHasMember ?person .
          	?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
          	BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph member_by_initial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
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
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?seatIncumbencyStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house ;
        :incumbencyStartDate ?houseIncumbencyStartDate ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?house
        a :House ;
        :houseName ?houseName .
    ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?partyMembershipStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        ?person a :Person ;
        :memberHasIncumbency ?incumbency .
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person :memberHasMemberImage ?image . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        { 
            ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency)
            ?houseIncumbency :houseIncumbencyHasHouse ?house .
            ?house :houseName ?houseName .
            ?houseIncumbency :incumbencyStartDate ?houseIncumbencyStartDate .
            OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        }
        UNION {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency 
                :seatIncumbencyHasHouseSeat ?houseSeat ;
                :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasHouse ?house .
            ?house :houseName ?houseName .
            OPTIONAL { 
                ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                ?constituencyGroup :constituencyGroupName ?constituencyName . 
            }
        }
        ?person :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership 
            :partyMembershipHasParty ?party ;
            :partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
        ?party :partyName ?partyName .
        FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
        }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {

            ?incumbency a :Incumbency ;
                        :incumbencyHasMember ?person.
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);

        }

        [HttpGet]
        public Graph member_a_to_z()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    [ :value ?firstLetter ]
}
WHERE {
    SELECT DISTINCT ?firstLetter 
    WHERE {
        ?Incumbency 
            a :Incumbency ;
            :incumbencyHasMember ?person .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph member_current_by_initial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
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
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?seatIncumbencyStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house ;
        :incumbencyStartDate ?houseIncumbencyStartDate ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?house
        a :House ;
        :houseName ?houseName .
    ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?partyMembershipStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        ?person 
            a :Person ;
            :memberHasIncumbency ?incumbency .
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person :memberHasMemberImage ?image . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        { 
            ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency)
            ?houseIncumbency :houseIncumbencyHasHouse ?house .
            ?house :houseName ?houseName .
            ?houseIncumbency :incumbencyStartDate ?houseIncumbencyStartDate .
            OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        }
        UNION {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
            ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasHouse ?house .
            ?house :houseName ?houseName .
            OPTIONAL {
                ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                ?constituencyGroup :constituencyGroupName ?constituencyName .
            }
        }
        ?person :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasParty ?party .
        ?party :partyName ?partyName .
        ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
        FILTER NOT EXISTS {?incumbency a :PastIncumbency}
        FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial)) .
        }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {

            ?incumbency a :Incumbency ;
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency.   }
            ?incumbency :incumbencyHasMember ?person.
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

           BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph member_current_a_to_z()
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
        FILTER NOT EXISTS { ?incumbency a :PastIncumbency .	}
        ?incumbency :incumbencyHasMember ?person .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }
    }
}
