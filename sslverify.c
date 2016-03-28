/***************************************************************************
  * Copyright (C) 1998 - 2016, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This software is licensed as described in the file COPYING, which
 * you should have received as part of this distribution. The terms
 * are also available at https://curl.haxx.se/docs/copyright.html.
 *
 * You may opt to use, copy, modify, merge, publish, distribute and/or sell
 * copies of the Software, and permit persons to whom the Software is
 * furnished to do so, under the terms of the COPYING file.
 *
 * This software is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY
 * KIND, either express or implied.
 *
 ***************************************************************************/ 
/* <DESC>
 * CA cert in memory with OpenSSL to get a HTTPS page.
 * </DESC>
 */ 
 
#include <openssl/ssl.h>
#include <curl/curl.h>
#include <stdio.h>
 
size_t writefunction( void *ptr, size_t size, size_t nmemb, void *stream)
{
  fwrite(ptr, size, nmemb, stream);
  return (nmemb*size);
}

static int curl_xferinfo_function( void *clienttp, curl_off_t dltot, curl_off_t dlnow, curl_off_t ultot, curl_off_t ulnow )
{
	printf("transfer progress: %d-%d-%d-%d\n", (int)dltot, (int)dlnow, (int)ultot, (int)ulnow);
	return CURLE_OK;
}
 
static CURLcode sslctx_function(CURL * curl, void * sslctx, void * parm)
{
  X509_STORE * store;
  X509 * cert=NULL;
  BIO * bio;
  char * mypem = /* www.cacert.org */ 
  "-----BEGIN CERTIFICATE-----\n"\
	"MIIDKDCCAhACCQDTjLfvVb5LwDANBgkqhkiG9w0BAQsFADBWMQswCQYDVQQGEwJh\n"\
	"dTELMAkGA1UECAwCc2ExDDAKBgNVBAcMA2FkZTELMAkGA1UECgwCbXkxCzAJBgNV\n"\
	"BAsMAm15MRIwEAYDVQQDDAlsb2NhbGhvc3QwHhcNMTYwMzI3MTMwODQ5WhcNNDMw\n"\
	"ODEyMTMwODQ5WjBWMQswCQYDVQQGEwJhdTELMAkGA1UECAwCc2ExDDAKBgNVBAcM\n"\
	"A2FkZTELMAkGA1UECgwCbXkxCzAJBgNVBAsMAm15MRIwEAYDVQQDDAlsb2NhbGhv\n"\
	"c3QwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCj2kmYaAzH8VzQjWBy\n"\
	"YgT42dR1qavqrk6adn91wIttxi3kPe9PsYIDFJIDaFCz6VMzpbQKWKVI1Ra3rLLE\n"\
	"1z/Rii5sMFDi6+VUmd8PYONnVd4iXAKK6dPwkqjvbZj3L4cKF1LqIUlQwpdfCpqn\n"\
	"Y2DPFcgU93yuy0tMpE7I84docKhKUChanvOJsrffKzG6r/JlUZrtMmNiOjPSRnKR\n"\
	"zvR5hEV1NeFsyvBrMb709tlJibJmmvDLEq84NveYhA6kFyPcUq+MEhe1sAH/8uJQ\n"\
	"1DT5UgHJMzx9qI7l6mKJUHLpi+58wgDBMtTlkRSD1Uh/vh1Rtg+OX2Zcztc3Ul4u\n"\
	"5b03AgMBAAEwDQYJKoZIhvcNAQELBQADggEBAHCmjl4nUXWqcvHXnY2Xsu8qDifh\n"\
	"Lek6g9YB1+9XfmQYrJi5crJiOZJk0xlQeumw9hvSQmJVIPAoemwZFgrkVBY2ie1b\n"\
	"456DwW12AnZVI1d3o4x4rnPZtZfh/33cVZP62qzENWLvU4La9iFXQFtHq9gnLcfw\n"\
	"xbR+cpqV55JjBnoaX+6b3oY4T4nk1r10UVNhUN7OF1GgD/qoDdv7F5GlAp2vvvvu\n"\
	"K/W6KcLvY8WsW3UmnrBTLNQnAoCP5wQhCassXFOtW8GHgFkQ04eYxPWd9Zs4hUfz\n"\
	"UNsl+FDTyOW9HgOPc2snMZ3MXALNO68teW+qKuysYSLVQdMxgpaIQ93mdYY=\n"\
	"-----END CERTIFICATE-----\n";
  /* get a BIO */ 
  bio=BIO_new_mem_buf(mypem, -1);
  /* use it to read the PEM formatted certificate from memory into an X509
   * structure that SSL can use
   */ 
  PEM_read_bio_X509(bio, &cert, 0, NULL);
  if(cert == NULL)
    printf("PEM_read_bio_X509 failed...\n");
  /* get a pointer to the X509 certificate store (which may be empty!) */ 
  store=SSL_CTX_get_cert_store((SSL_CTX *)sslctx);
 
  /* add our certificate to this store */ 
  if(X509_STORE_add_cert(store, cert)==0)
    printf("error adding certificate\n");
 
  /* decrease reference counts */ 
  X509_free(cert);
  BIO_free(bio);
  /* all set to go */ 
  return CURLE_OK;
}
 
int main(void)
{
  CURL * ch;
  CURLcode rv;
  
  struct myprogress {
  	double lastruntime;
  	CURL *curl;
	} prog;
  
  rv=curl_global_init(CURL_GLOBAL_ALL);
  ch=curl_easy_init();
  rv=curl_easy_setopt(ch, CURLOPT_VERBOSE, 0L);
  rv=curl_easy_setopt(ch, CURLOPT_HEADER, 0L);
  rv=curl_easy_setopt(ch, CURLOPT_NOPROGRESS, 0L);
  rv=curl_easy_setopt(ch, CURLOPT_NOSIGNAL, 1L);
  rv=curl_easy_setopt(ch, CURLOPT_WRITEFUNCTION, *writefunction);
  rv=curl_easy_setopt(ch, CURLOPT_WRITEDATA, stdout);
  rv=curl_easy_setopt(ch, CURLOPT_HEADERFUNCTION, *writefunction);
  rv=curl_easy_setopt(ch, CURLOPT_HEADERDATA, stderr);
  rv=curl_easy_setopt(ch, CURLOPT_SSLCERTTYPE, "PEM");
  rv=curl_easy_setopt(ch, CURLOPT_SSL_VERIFYPEER, 1L);
  rv=curl_easy_setopt(ch, CURLOPT_SSL_VERIFYHOST, 2L);
  rv=curl_easy_setopt(ch, CURLOPT_URL, "https://localhost:8000/");
 
 	/* curl version */
 	printf("libcurl version: %x\n", LIBCURL_VERSION_NUM );
	 
  /* first try: retrieve page without cacerts' certificate -> will fail
   */ 
  rv=curl_easy_perform(ch);
  if(rv==CURLE_OK)
    printf("*** transfer succeeded ***\n %d", rv);
  else
    printf("*** transfer failed ***\n %d \n", rv);
 
  /* second try: retrieve page using cacerts' certificate -> will succeed
   * load the certificate by installing a function doing the nescessary
   * "modifications" to the SSL CONTEXT just before link init
   */ 
  rv=curl_easy_setopt(ch, CURLOPT_SSL_CTX_FUNCTION, *sslctx_function);
  rv=curl_easy_setopt(ch, CURLOPT_XFERINFOFUNCTION, *curl_xferinfo_function);
  rv=curl_easy_setopt(ch, CURLOPT_XFERINFODATA, &prog);
  rv=curl_easy_perform(ch);
  if(rv==CURLE_OK)
    printf("*** transfer succeeded ***\n");
  else
    printf("*** transfer failed ***\n %d \n", rv);
    
  rv=curl_easy_setopt(ch, CURLOPT_URL, "https://localhost:8000/content");
 	rv=curl_easy_perform(ch);
  if(rv==CURLE_OK)
    printf("*** transfer succeeded ***\n");
  else
    printf("*** transfer failed ***\n %d \n", rv);
    
  curl_easy_cleanup(ch);
  curl_global_cleanup();
  return rv;
}


