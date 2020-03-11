//Customer Insight
//window.demographicsMale = [21, 2, 12, 13, 38, 27, 8];
//window.demographicsFemale = [-46, -1, -28, -40, -80, -56, -26];
window.proximityData = [35, 44, 52, 128, 112, 55];
window.customerTypeData = [100, 69, 71, 34, 74, 20];
window.incomeData = [8, 16, 20, 62, 99, 184];
window.educationData = [61, 159, 105, 19, 25];


//Calendar
//window.dateCalendarHighlight = [];
function isContainBlockedDate(date) {
    if (window.dateDisabled) {
        for (var i = 0; i < window.dateDisabled.length; i++) {
            var disabledDate = new Date(window.dateDisabled[i]);
            if (disabledDate.equalDate(date)) {
                return true;
            }
        }
    }
    return false;
}
//var addBlockedDate = 0;
//for (var i = 1; i <= window.notAllowChangeStateDay; i++) {
//    var hightlightDate = new Date().addDays(i + addBlockedDate);
//    while (isContainBlockedDate(hightlightDate)) {
//        addBlockedDate += 1;
//        hightlightDate = new Date().addDays(i + addBlockedDate);
//    }
//    window.dateCalendarHighlight.push(hightlightDate);
//}