
# JQUERY QUERY BUILDER

Website URL : https://querybuilder.js.org/

## Overview
	QueryBuilder is an UI component to create queries and filters.
	It can be used on advanced search engine pages, administration backends, etc.
	It is highly customizable and is pluggable to many jQuery widgets like autocompleters, sliders and datepickers.
	It outputs a structured JSON of rules which can be easily parsed to create SQL/NoSQL/whatever queries.
	And it also comes with a set of great plugins and has a full events system for even more features.
	
QueryBuilder File Path: 	D:\Development\IDMS_NG\src\Infogroup.IDMS.Web.Host\ThirdPartyFiles\jQuery-QueryBuilder\query-builder.standalone.js

## Backend Implementation  
	
	
#### App Service : SegmentSelectionsAppService
	1. CreateSegmentSelectionDetails - used to process and save selections in databse
	2. ProcessZipRadius - used to process ZipRadius while saving
	3. SaveFileZipRadius - used to save ZipRadius changes after processing is done
	4. GetSelectionFieldsNew - used to get all the selection fields so that they can be bindede to rules dropdown in Query Builder
	5. GetDefaultColumnDefinitionsForType - used to fetch all the data related to fields
	6. GetFieldValues - used to get field(for G Type) values for a particular rule
		
## Frontend Implementation 
	
		
#### Angular Component- SelectionComponent
	1. saveWhereClause - used to call getSQLtblsegmentSelection function in query builder and execute save 
	2. saveFilters - used to save filter and subscribe to the APi call for CreateSegmentSelectionDetails method
	3. executeAction - used to trigger ececute command on selection screen
	4. selectAction - used to open different screens/popup-model available on selection screen
	5. BindBuilder - used to fetch all the information related to fields and provide them to Query builder for binding
		
#### Query-builder.standalone.js
		
	1. setRulesFromSQL - used to set/bind fields/rules to dropdown in query builder
	2. addGroup - used to add a new group in the UI
	3. deleteGroup - used to delete a group and the rules inside it from UI
	4. updateGroupCondition - used to update the condition(AND/OR) for rules
	5. addRule - used to add a new rule to the UI
	6. deleteRule - used to delete a rule from the UI
	7. forFileUpload - used to upload a file for F type rules
	8. createRuleOperators - used to add/create operators dropdown
	9. createRuleInput - used to render/add input element for a rule
	10. updateRuleFilter - used to update the rule/filter value
	11. updateRuleOperator - used to update operator value
	12. updateRuleValue - used to update any chabge in rule input value
	13. reset- used to reset query builder to default form
	14. getSegmentSelectionRules - used to process rules and create an object with all the rules when saving
	15. validateZipRadius - contains Zip Radius validations
	16. validateGeoRadius - contains Geo Radius validations
	17. validateOperators - contains validations for operators
	18. setRules - used to bind rules/fields to the rule dropdown
	19. getOperators - used to fetch operator for a rule
	20. setRuleInputValue - used to set input value for a rule
	21. getRuleInputValue - returns input value for a rule
	22. QueryBuilder.templates - contains the template of query builder
	23. getGroupTemplate - parses group template
	24. getRuleTemplate - parses rule template
		
#### Change detection in Query builder

	Reason for implementation: 
	Change detection was implemented to identify the rules which were gettting updated through the UI so that value for that particular rule can be updated in Database.

	Points to note:		
	1. Variable used for each rule : isRuleUpdated
		  value 0 : Newly created
			    1 : Change detected
	    rule <id> : No change detected
			
	2. To detect if any change was done or not for a particular rule, we are modifying the value of 'isRuleUpdated' to 1 on functions which are updating data for a rule like updateGroupCondition, forFileUpload, updateRuleFilter, updateRuleOperator, updateRuleValue, setRuleInputValue, setRules
			
	Logic used:
		item.isRuleUpdated = item.isRuleUpdated === 0 ? item.isRuleUpdated : 1;
				
	3. When a rule is deleted, rule's id is added to variable 'deletedSelections' and later passed to the backend for deletion of rules.
		
		
		
	
  