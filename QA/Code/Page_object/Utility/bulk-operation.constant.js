var Bulkoperationconstant = function () {
    this.bulkOperation_actions = ['Add To Favorites', 'Append Rules', 'Campaign History', 'Delete Rules', 'Delete Segments', 'Edit Segments', 'Find & Replace'];
    this.bulkOperation_editSegmentGrid = ['Edit Segments', '#', 'Description', 'Required Qty', 'Provided Qty', 'Available Qty', 'Key Code'];
    this.bulkOperation_fieldLabels = ['Search For *', 'Replace With', 'Field Description'];
    this.bulkOperation_inlineGrid = ['Edit Segments', '#', 'Description', 'Required Qty', 'Key Code 1', 'Max Per', 'Net Group', 'Random Radius Nth', 'Key Code 2'];
    this.bulkOperation_inlineGridOutputQty = ['Edit Segments', '#', 'Description', 'Required Qty', 'Output Qty', 'Key Code 1', 'Max Per', 'Net Group', 'Random Radius Nth', 'Key Code 2'];
    this.bulkOperation_toggleButton = 'Inline Edit';
    this.bulkOperation_totalCountValues = ['Total: 1', 'Total: 2'];
    this.bulkOperation_segments = ['1', '1-2'];
    this.selection_actions = ['Campaign - Edit', 'Campaign - History', 'Campaign - Print', '', 'Segment - Add', 'Segment - Copy', 'Segment - Edit', 'Segment - Delete', 'Segment - Import', 'Segment - Sources'];
    this.selection_addSegment = ['Add Segment', '#', 'Description *', 'Required Quantity *', 'Key Code 1', 'Key Code 2', 'Max Per', 'Output Quantity', 'Net Group # *', 'Random Radius Nth'];
    this.createSegmentValues = {
        description: "Automation Segment Test",
        requiredQty: "1000",
        keyCode1: "Keycode1 Test",
        keyCode2: "Keycode2 Test",
        maxPer: "05",
        netGroup: "10",
    };
    this.editSegmentValues = {
        title: 'Edit Segment',
        segmentNo: '2',
        description: "Automation Edit Segment Test",
        requiredQty: "2000",
        keyCode1: "Keycode1 Edit",
        keyCode2: "Keycode2 Edit",
        maxPer: "06",
        netGroup: "11",
    };
};
module.exports = Bulkoperationconstant;