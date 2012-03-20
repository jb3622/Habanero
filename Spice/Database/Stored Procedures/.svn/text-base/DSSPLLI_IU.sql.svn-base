 CREATE PROCEDURE IPTSQUADSY/DSSPLLI_IU                             
 (IN P_CLASS DEC (4, 0),                                       
  IN P_VENDOR DEC (5, 0),                                      
  IN P_STYLE DEC (4, 0),                                       
  IN P_COLOUR DEC (3, 0),                                      
  IN P_SIZE DEC (4, 0), 
  IN P_LANG CHAR (12),                                         
  IN P_DESC  GRAPHIC (30) CCSID 13488 )                        
 LANGUAGE SQL                                                  
 MODIFIES SQL DATA                                             
 BEGIN                                                         
  IF EXISTS ( SELECT * FROM DSSPILD WHERE ILDCLS=P_CLASS AND   
  ILDVEN=P_VENDOR AND ILDSTY=P_STYLE AND ILDCLR=P_COLOUR AND   
  ILDLAN=P_LANG AND ILDSIZ=P_SIZE) THEN                                         
                                                               
  UPDATE DSSPILD SET ILDDES = P_DESC  WHERE ILDCLS=P_CLASS AND 
  ILDVEN=P_VENDOR AND ILDSTY=P_STYLE AND ILDCLR=P_COLOUR AND   
  ILDLAN=P_LANG AND ILDSIZ=P_SIZE;                                                   
                                                                  
 ELSE                                                             
   INSERT INTO DSSPILD                                            
  (ILDCLS,ILDVEN,ILDCLR,ILDSTY,ILDLAN,ILDDES,ILDSIZ)                     
   VALUES (P_CLASS,P_VENDOR,P_COLOUR,P_STYLE,P_LANG,P_DESC,P_SIZE);      
 END IF;                                                          
                                                                  
END