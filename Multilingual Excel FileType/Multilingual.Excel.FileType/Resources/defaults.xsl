<!--
 |
 | XSLT REC Compliant Version of IE5 Default Stylesheet
 |
 | Original version by Jonathan Marsh (jmarsh@xxxxxxxxxxxxx)
 | http://msdn.microsoft.com/xml/samples/defaultss/defaultss.xsl
 |
 | Conversion to XSLT 1.0 REC Syntax by Steve Muench (smuench@xxxxxxxxxx)
 |
 +-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- Add doctype attributes to keep IE happy -->
  <xsl:output indent="no"
                method="html"
                doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN"
                doctype-system="http://www.w3.org/tr/xhtml1/DTD/xhtml1-transitional.dtd" />
  <xsl:template match="/">
    <HTML>
      <HEAD>
        <SCRIPT>
          <xsl:comment>
            <![CDATA[
                  function f(e){
                     if (e.className=="ci") {
                       if (e.children(0).innerText.indexOf("\n")>0) fix(e,"cb");
                     }
                     if (e.className=="di") {
                       if (e.children(0).innerText.indexOf("\n")>0) fix(e,"db");
                     } e.id="";
                  }
                  function fix(e,cl){
                    e.className=cl;
                    e.style.display="block";
                    j=e.parentElement.children(0);
                    j.className="c";
                    k=j.children(0);
                    k.style.visibility="visible";
                    k.href="#";
                  }
                  function ch(e) {
                    mark=e.children(0).children(0);
                    if (mark.innerText=="+") {
                      mark.innerText="-";
                      for (var i=1;i<e.children.length;i++) {
                        e.children(i).style.display="block";
                      }
                    }
                    else if (mark.innerText=="-") {
                      mark.innerText="+";
                      for (var i=1;i<e.children.length;i++) {
                        e.children(i).style.display="none";
                      }
                    }
                  }
                  function ch2(e) {
                    mark=e.children(0).children(0);
                    contents=e.children(1);
                    if (mark.innerText=="+") {
                      mark.innerText="-";
                      if (contents.className=="db"||contents.className=="cb") {
                        contents.style.display="block";
                      }
                      else {
                        contents.style.display="inline";
                      }
                    }
                    else if (mark.innerText=="-") {
                      mark.innerText="+";
                      contents.style.display="none";
                    }
                  }
                  function cl() {
                    e=window.event.srcElement;
                    if (e.className!="c") {
                      e=e.parentElement;
                      if (e.className!="c") {
                        return;
                      }
                    }
                    e=e.parentElement;
                    if (e.className=="e") {
                      ch(e);
                    }
                    if (e.className=="k") {
                      ch2(e);
                    }
                  }
                  function ex(){}
                  function h(){window.status=" ";}

                  document.onclick=cl;				  		
					
				         function setActiveStyle(objDivID){					
					        var n = document.getElementById(objDivID);
					        var a = [];
					        var i;
					        while(n) {
						        a.push(n);
						        n.id = "a-different-id";
						        n = document.getElementById(objDivID);
					        }
						
					        for (var i = 0; i < a.length; i++) {
						        a[i].className = 'activesegment';
						        a[i].id = objDivID
					        }									
				         }
				 
				         function setNormalStyle(objDivID){				
					        var n = document.getElementById(objDivID);
					        var a = [];
					        var i;
					        while(n) {
						        a.push(n);
						        n.id = "a-different-id";
						        n = document.getElementById(objDivID);
					        }
						
					        for (var i = 0; i < a.length; i++) {
						        a[i].className = 'normal';
						        a[i].id = objDivID
					        }											
				         }
              ]]>
          </xsl:comment>
        </SCRIPT>
        <STYLE>
          BODY {font:small 'Verdana'; margin-right:1.5em; margin-top: 10px;}
          .c  {cursor:hand}
          .b  {color:#000000}
          .e  {margin-left:1em; text-indent:-1em; margin-right:1em}
          .k  {margin-left:1em; text-indent:-1em; margin-right:1em}
          .t  {color:#990000}
          .xt {color:#990099}
          .ns {color:red}
          .dt {color:green}
          .m  {color:blue}
          .tx {font-weight:bold}
          .db {text-indent:0px; margin-left:1em; margin-top:0px;
          margin-bottom:0px;padding-left:.3em;
          border-left:1px solid #CCCCCC; font:x-small Courier}
          .di {font:x-small Courier}
          .d  {color:blue}
          .pi {color:blue}
          .cb {text-indent:0px; margin-left:1em; margin-top:0px;
          margin-bottom:0px;padding-left:.3em; font:x-small Courier;
          color:#888888}
          .ci {font:x-small Courier; color:#888888}
          PRE {margin:0px; display:inline}

          .label {padding-left:20px; vertical-align: middle}
          .validation {color: white; padding: 3px; margin: 5px 5px 5px 5px; text-indent: 0}
          .summary {position: fixed; top: 0; left: 0; margin: 0px; padding-top: 10px; width: 100%; height: 32px; font-size: 12pt; vertical-align: middle;border-bottom: 0px solid black}
          .nav {float: right; padding-right:20px;}
          .failure {background: red;}
          .success {background: green;}
          .warning {background: yellow; color: black}
          .selected {font-weight: bold; text-indent: 1em}
		  	
		      span{color:black;cursor: hand;}
		      .activesegment{background-color: #aad6ac !important}
		      .normal{color:black;background-color: #fcffa4;}
		  
        </STYLE>
      </HEAD>
      <BODY class="st">
        <xsl:apply-templates/>
      </BODY>
    </HTML>
  </xsl:template>

  <!-- Render the schema summary
         Include jquery from CDN and render a title bar across top -->
  <xsl:template match="processing-instruction('schemaSummary')">
    <script src="http://code.jquery.com/jquery-1.9.1.min.js"></script>
    <div id="schemaSummary" class="validation summary">
      <span class="label">
        <xsl:value-of select="." />
      </span>
      <span class="nav">
        <button id="prev">&lt;&lt;</button>
        <button id="next">>></button>
      </span>
    </div>
  </xsl:template>

  <!-- Handle the schemaValid processing instruction 
         This sets up a document.ready routine that 
         colour codes a visual queue i.e. a green title bar.
    -->
  <xsl:template match="processing-instruction('schemaValid')">
    <script>
      $(document).ready(function() {
      $("#schemaSummary").addClass("success")
      $("button").css("display", "none");
      });
    </script>
  </xsl:template>

  <!-- Handle the schemaInvalid processing instruction
         Renders a red title bar with navigation controls to
         step through the errors.    
     -->
  <xsl:template match="processing-instruction('schemaInvalid')">
    <script>
      <![CDATA[
            var index = -1;
            var errors = $("div.failure");
            var offsetFromTop = $("#schemaSummary").outerHeight();
            function nextError(){
                if(index > -1) 
                    $(errors).eq(index).removeClass("selected");
                index ++;
                if(index >= $(errors).size())
                    index = 0;

                $(errors).eq(index).addClass("selected");                                       
                scrollTo($(errors).eq(index), offsetFromTop);
            }
            function prevError(){
                $(errors).eq(index).removeClass("selected");
                index --;
                if(index < 0)
                    index = $(errors).size() - 1;

                $(errors).eq(index).addClass("selected");                                       
                scrollTo($(errors).eq(index), offsetFromTop);               
            }
            function scrollTo(element, offsetFromTop) {
                $('html,body').animate({scrollTop: $(element).offset().top - offsetFromTop},'fast');
            }

            $(document).ready(function() {
                $("#schemaSummary").addClass("warning");
                $("#next").click(function() {
                    nextError();
                });
                $("#prev").click(function() {
                    prevError();
                });
            });
            ]]>
    </script>
  </xsl:template>

  <!-- Add a colour coded bar in situ i.e. where the validation
         error has occured -->
  <xsl:template match="processing-instruction('error')">
    <div class="validation failure">
      <xsl:value-of select="."></xsl:value-of>
    </div>
  </xsl:template>

  <xsl:template match="processing-instruction()">
    <DIV class="e">
      <SPAN class="b">
        <xsl:call-template name="entity-ref">
          <xsl:with-param name="name">nbsp</xsl:with-param>
        </xsl:call-template>
      </SPAN>
      <SPAN class="m">
        <xsl:call-template name="entity-ref">
          <xsl:with-param name="name">lt</xsl:with-param>
        </xsl:call-template>?
      </SPAN>
      <SPAN class="pi">
        <xsl:value-of select="name(.)"/>
      </SPAN>
      <SPAN>
        <xsl:call-template name="entity-ref">
          <xsl:with-param name="name">nbsp</xsl:with-param>
        </xsl:call-template>
        <xsl:value-of select="."/>
      </SPAN>
      <SPAN class="m">
        <xsl:text>?></xsl:text>
      </SPAN>
    </DIV>
  </xsl:template>
  <xsl:template match="processing-instruction('xml')">
    <DIV class="e">
      <SPAN class="b">
        <xsl:call-template name="entity-ref">
          <xsl:with-param name="name">nbsp</xsl:with-param>
        </xsl:call-template>
      </SPAN>
      <SPAN class="m">
        <xsl:call-template name="entity-ref">
          <xsl:with-param name="name">lt</xsl:with-param>
        </xsl:call-template>?
      </SPAN>
      <SPAN class="pi">
        <xsl:text>xml </xsl:text>
        <xsl:for-each select="@*">
          <xsl:value-of select="name(.)"/>
          <xsl:text>="</xsl:text>
          <xsl:value-of select="."/>
          <xsl:text>" </xsl:text>
        </xsl:for-each>
      </SPAN>
      <SPAN class="m">
        <xsl:text>?></xsl:text>
      </SPAN>
    </DIV>
  </xsl:template>
  <xsl:template match="@*">
    <SPAN>
      <xsl:attribute name="class">
        <xsl:if test="xsl:*/@*">
          <xsl:text>x</xsl:text>
        </xsl:if>
        <xsl:text>t</xsl:text>
      </xsl:attribute>
      <xsl:value-of select="name(.)"/>
    </SPAN>
    <SPAN class="m">="</SPAN>
    <B>
      <xsl:value-of select="."/>
    </B>
    <SPAN class="m">"</SPAN>
  </xsl:template>
  <xsl:template match="text()">
    <DIV class="e">
      <SPAN class="b"> </SPAN>
      <SPAN class="tx">
        <xsl:value-of select="."/>
      </SPAN>
    </DIV>
  </xsl:template>
  <xsl:template match="comment()">
    <DIV class="k">
      <SPAN>
        <A STYLE="visibility:hidden" class="b" onclick="return false" onfocus="h()">-</A>
        <SPAN class="m">
          <xsl:call-template name="entity-ref">
            <xsl:with-param name="name">lt</xsl:with-param>
          </xsl:call-template>!--
        </SPAN>
      </SPAN>
      <SPAN class="ci" id="clean">
        <PRE>
          <xsl:value-of select="."/>
        </PRE>
      </SPAN>
      <SPAN class="b">
        <xsl:call-template name="entity-ref">
          <xsl:with-param name="name">nbsp</xsl:with-param>
        </xsl:call-template>
      </SPAN>
      <SPAN class="m">
        <xsl:text>--></xsl:text>
      </SPAN>
      <SCRIPT>f(clean);</SCRIPT>
    </DIV>
  </xsl:template>
  <xsl:template match="*">
    <DIV class="e">
      <DIV STYLE="margin-left:1em;text-indent:-2em">
        <SPAN class="b">
          <xsl:call-template name="entity-ref">
            <xsl:with-param name="name">nbsp</xsl:with-param>
          </xsl:call-template>
        </SPAN>
        <SPAN class="m">
          <xsl:call-template name="entity-ref">
            <xsl:with-param name="name">lt</xsl:with-param>
          </xsl:call-template>
        </SPAN>
        <SPAN>
          <xsl:attribute name="class">
            <xsl:if test="xsl:*">
              <xsl:text>x</xsl:text>
            </xsl:if>
            <xsl:text>t</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="name(.)"/>
          <xsl:if test="@*">
            <xsl:text> </xsl:text>
          </xsl:if>
        </SPAN>
        <xsl:apply-templates select="@*"/>
        <SPAN class="m">
          <xsl:text>/></xsl:text>
        </SPAN>
      </DIV>
    </DIV>
  </xsl:template>
  <xsl:template match="*[node()]">
    <DIV class="e">
      <DIV class="c">
        <A class="b" href="#" onclick="return false" onfocus="h()">-</A>
        <SPAN class="m">
          <xsl:call-template name="entity-ref">
            <xsl:with-param name="name">lt</xsl:with-param>
          </xsl:call-template>
        </SPAN>
        <SPAN>
          <xsl:attribute name="class">
            <xsl:if test="xsl:*">
              <xsl:text>x</xsl:text>
            </xsl:if>
            <xsl:text>t</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="name(.)"/>
          <xsl:if test="@*">
            <xsl:text> </xsl:text>
          </xsl:if>
        </SPAN>
        <xsl:apply-templates select="@*"/>
        <SPAN class="m">
          <xsl:text>></xsl:text>
        </SPAN>
      </DIV>
      <DIV>
        <xsl:apply-templates/>
        <DIV>
          <SPAN class="b">
            <xsl:call-template name="entity-ref">
              <xsl:with-param name="name">nbsp</xsl:with-param>
            </xsl:call-template>
          </SPAN>
          <SPAN class="m">
            <xsl:call-template name="entity-ref">
              <xsl:with-param name="name">lt</xsl:with-param>
            </xsl:call-template>?/
          </SPAN>
          <SPAN>
            <xsl:attribute name="class">
              <xsl:if test="xsl:*">
                <xsl:text>x</xsl:text>
              </xsl:if>
              <xsl:text>t</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="name(.)"/>
          </SPAN>
          <SPAN class="m">
            <xsl:text>></xsl:text>
          </SPAN>
        </DIV>
      </DIV>
    </DIV>
  </xsl:template>
  <xsl:template match="*[text() and not (comment() or processing-instruction())]">
    <DIV class="e">
      <DIV STYLE="margin-left:1em;text-indent:-2em">
        <SPAN class="b">
          <xsl:call-template name="entity-ref">
            <xsl:with-param name="name">nbsp</xsl:with-param>
          </xsl:call-template>
        </SPAN>
        <SPAN class="m">
          <xsl:call-template name="entity-ref">
            <xsl:with-param name="name">lt</xsl:with-param>
          </xsl:call-template>
        </SPAN>
        <SPAN>
          <xsl:attribute name="class">
            <xsl:if test="xsl:*">
              <xsl:text>x</xsl:text>
            </xsl:if>
            <xsl:text>t</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="name(.)"/>
          <xsl:if test="@*">
            <xsl:text> </xsl:text>
          </xsl:if>
        </SPAN>
        <xsl:apply-templates select="@*"/>
        <SPAN class="m">
          <xsl:text>></xsl:text>
        </SPAN>
        <SPAN class="tx">
          <xsl:value-of select="."/>
        </SPAN>
        <SPAN class="m">
          <xsl:call-template name="entity-ref">
            <xsl:with-param name="name">lt</xsl:with-param>
          </xsl:call-template>/
        </SPAN>
        <SPAN>
          <xsl:attribute name="class">
            <xsl:if test="xsl:*">
              <xsl:text>x</xsl:text>
            </xsl:if>
            <xsl:text>t</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="name(.)"/>
        </SPAN>
        <SPAN class="m">
          <xsl:text>></xsl:text>
        </SPAN>
      </DIV>
    </DIV>
  </xsl:template>
  <xsl:template match="*[*]" priority="20">
    <DIV class="e">
      <DIV STYLE="margin-left:1em;text-indent:-2em" class="c">
        <A style="text-decoration:none" class="b" href="#" onclick="return false" onfocus="h()">-</A>
        <SPAN class="m">
          <xsl:call-template name="entity-ref">
            <xsl:with-param name="name">lt</xsl:with-param>
          </xsl:call-template>
        </SPAN>
        <SPAN>
          <xsl:attribute name="class">
            <xsl:if test="xsl:*">
              <xsl:text>x</xsl:text>
            </xsl:if>
            <xsl:text>t</xsl:text>
          </xsl:attribute>
          <xsl:value-of select="name(.)"/>
          <xsl:if test="@*">
            <xsl:text> </xsl:text>
          </xsl:if>
        </SPAN>
        <xsl:apply-templates select="@*"/>
        <SPAN class="m">
          <xsl:text>></xsl:text>
        </SPAN>
      </DIV>
      <DIV>
        <xsl:apply-templates/>
        <DIV>
          <SPAN class="b">
            <xsl:call-template name="entity-ref">
              <xsl:with-param name="name">nbsp</xsl:with-param>
            </xsl:call-template>
          </SPAN>
          <SPAN class="m">
            <xsl:call-template name="entity-ref">
              <xsl:with-param name="name">lt</xsl:with-param>
            </xsl:call-template>/
          </SPAN>
          <SPAN>
            <xsl:attribute name="class">
              <xsl:if test="xsl:*">
                <xsl:text>x</xsl:text>
              </xsl:if>
              <xsl:text>t</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="name(.)"/>
          </SPAN>
          <SPAN class="m">
            <xsl:text>></xsl:text>
          </SPAN>
        </DIV>
      </DIV>
    </DIV>
  </xsl:template>

  <xsl:template name="entity-ref">
    <xsl:param name="name"/>
    <xsl:text disable-output-escaping="yes">&amp;</xsl:text>
    <xsl:value-of select="$name"/>
    <xsl:text>;</xsl:text>
  </xsl:template>
</xsl:stylesheet>
