$.fn.ignore = function(sel){
  return this.clone().find(sel||">*").remove().end();
};